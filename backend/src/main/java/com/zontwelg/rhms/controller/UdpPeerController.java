package com.zontwelg.rhms.controller;

import com.zontwelg.rhms.domain.ResponseContainer;
import com.zontwelg.rhms.domain.UdpSession;
import com.zontwelg.rhms.service.IUdpSessionsService;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.*;

import javax.servlet.http.HttpServletRequest;
import javax.xml.ws.Response;
import java.net.InetSocketAddress;
import java.util.List;

@RestController
@RequestMapping("/udp-peer")
public class UdpPeerController extends AbstractController {

    private static final Logger log = LoggerFactory.getLogger("application");

    @Autowired
    IUdpSessionsService udpSessionsRepository;

    @GetMapping("/register")
    public ResponseEntity<ResponseContainer> processSessionRequest(@RequestParam("name") String peerName,
                                                                   @RequestParam("port") int natPort,
                                                                   HttpServletRequest request)
    {
        InetSocketAddress remoteUser = InetSocketAddress.createUnresolved(request.getRemoteAddr(), natPort);
        UdpSession activeInstance = udpSessionsRepository.findByAddress(remoteUser);
        if (activeInstance != null){
            udpSessionsRepository.updateSpecificSession(activeInstance, peerName);
            ResponseContainer<String> container = new ResponseContainer<>();
            container.setBody("Already registered");

            HttpHeaders responseHeaders = new HttpHeaders();
            responseHeaders.setContentType(MediaType.APPLICATION_JSON);

            return new ResponseEntity<>(container, responseHeaders, HttpStatus.OK);
        }

        activeInstance = udpSessionsRepository.registerSession(remoteUser, peerName);
        if (activeInstance == null){
            log.warn("Registered session for peer [ip={}, port={}, name={}] is failed, service return null",
                    request.getRemoteAddr(),
                    natPort,
                    peerName);

            ResponseContainer container = new ResponseContainer<>();
            container.addError(makeError(HttpStatus.INTERNAL_SERVER_ERROR.value(), "Registered sessions returns as null"));

            HttpHeaders responseHeaders = new HttpHeaders();
            responseHeaders.setContentType(MediaType.APPLICATION_JSON);

            return new ResponseEntity<>(container, responseHeaders, HttpStatus.INTERNAL_SERVER_ERROR);
        }

        log.info("Registered session for peer [ip={}, port={}, name={}] is successful",
                request.getRemoteAddr(),
                natPort,
                peerName);

        ResponseContainer<String> container = new ResponseContainer<>();
        container.setBody("Successfully registered");

        HttpHeaders responseHeaders = new HttpHeaders();
        responseHeaders.setContentType(MediaType.APPLICATION_JSON);

        return new ResponseEntity<>(container, responseHeaders, HttpStatus.OK);
    }

    @GetMapping("/ping")
    public ResponseEntity<ResponseContainer> processPingRequest(@RequestParam("port") int natPort,
                                                                HttpServletRequest request)
    {
        InetSocketAddress remoteUser = InetSocketAddress.createUnresolved(request.getRemoteAddr(), natPort);
        UdpSession activeSession = udpSessionsRepository.findByAddress(remoteUser);
        if (activeSession == null){
            return responseSessionExpiredOrNotExists();
        }
        activeSession.updateKeepAlive();

        ResponseContainer<String> container = new ResponseContainer<>();
        container.setBody("Session updated");
        HttpHeaders responseHeaders = new HttpHeaders();
        responseHeaders.setContentType(MediaType.APPLICATION_JSON);

        return new ResponseEntity<>(container, responseHeaders, HttpStatus.OK);
    }

    @GetMapping("/get-active")
    public ResponseEntity<ResponseContainer> processListPeersByName(@RequestParam("name") String peerName)
    {
        UdpSession session = udpSessionsRepository.findByPeerName(peerName);
        if (session == null){
            ResponseContainer container = new ResponseContainer<>();
            container.addError(makeError(HttpStatus.INTERNAL_SERVER_ERROR.value(), "Session by that peer name is not found"));

            HttpHeaders responseHeaders = new HttpHeaders();
            responseHeaders.setContentType(MediaType.APPLICATION_JSON);

            return new ResponseEntity<>(container, responseHeaders, HttpStatus.INTERNAL_SERVER_ERROR);
        }

        ResponseContainer<UdpSession> container = new ResponseContainer<>();
        container.setBody(session);
        HttpHeaders responseHeaders = new HttpHeaders();
        responseHeaders.setContentType(MediaType.APPLICATION_JSON);

        return new ResponseEntity<>(container, responseHeaders, HttpStatus.OK);
    }

    @GetMapping("/list")
    public ResponseEntity<ResponseContainer> processListAllPeers() {
        List<UdpSession> sessions = udpSessionsRepository.getAll();

        ResponseContainer<List<UdpSession>> container = new ResponseContainer<>();
        container.setBody(sessions);
        HttpHeaders responseHeaders = new HttpHeaders();
        responseHeaders.setContentType(MediaType.APPLICATION_JSON);

        return new ResponseEntity<>(container, responseHeaders, HttpStatus.OK);
    }

    @GetMapping("/connect")
    public ResponseEntity<ResponseContainer> processPeerConnection(@RequestParam("name") String peerName,
                                                                   @RequestParam("ip") String peerIp,
                                                                   @RequestParam("peerPort") int peerPort)
    {
        UdpSession session = udpSessionsRepository.findByPeerNameAndAddress(peerIp, peerPort, peerName);
        if (session == null){
            return responseSessionExpiredOrNotExists();
        }

        ResponseContainer<String> container = new ResponseContainer<>();
        container.setBody("Connection initiated");
        HttpHeaders responseHeaders = new HttpHeaders();
        responseHeaders.setContentType(MediaType.APPLICATION_JSON);

        return new ResponseEntity<>(container, responseHeaders, HttpStatus.OK);
    }

    public ResponseEntity<ResponseContainer> responseSessionExpiredOrNotExists(){
        ResponseContainer container = new ResponseContainer<>();
        container.addError(makeError(HttpStatus.EXPECTATION_FAILED.value(), "Session expired or never exists"));

        HttpHeaders responseHeaders = new HttpHeaders();
        responseHeaders.setContentType(MediaType.APPLICATION_JSON);

        return new ResponseEntity<>(container, responseHeaders, HttpStatus.EXPECTATION_FAILED);
    }
}
