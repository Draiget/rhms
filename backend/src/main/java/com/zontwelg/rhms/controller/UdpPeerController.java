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
                                                                   @RequestParam(value = "ip", required = false) String publicIp,
                                                                   @RequestParam("port") String natPort,
                                                                   @RequestParam(value = "privatePort", required = false) String privatePort,
                                                                   HttpServletRequest request)
    {
        if (publicIp == null || publicIp.isEmpty()){
            publicIp = request.getHeader("X-FORWARDED-FOR");
            if (publicIp == null || publicIp.isEmpty()){
                publicIp = request.getRemoteAddr();
            }
        }

        InetSocketAddress remoteUser = InetSocketAddress.createUnresolved(publicIp, Integer.parseInt(natPort));
        UdpSession activeSession = udpSessionsRepository.findByAddress(remoteUser);
        if (activeSession != null){
            udpSessionsRepository.updateSpecificSession(activeSession, peerName, privatePort);
            ResponseContainer<String> container = new ResponseContainer<>();
            container.setBody("Already registered");

            HttpHeaders responseHeaders = new HttpHeaders();
            responseHeaders.setContentType(MediaType.APPLICATION_JSON);

            return new ResponseEntity<>(container, responseHeaders, HttpStatus.OK);
        }

        activeSession = udpSessionsRepository.registerSession(remoteUser, peerName);
        if (privatePort != null && !privatePort.isEmpty()) {
            try {
                activeSession.privatePort = Integer.parseInt(privatePort);
            } catch (NumberFormatException e) {
                activeSession.privatePort = 0;
            }
        }
        if (activeSession == null){
            log.warn("Registered session for peer [ip={}, port={}, name={}] is failed, service return null",
                    activeSession.publicAddress.getHostName(),
                    activeSession.publicAddress.getPort(),
                    peerName);

            ResponseContainer container = new ResponseContainer<>();
            container.addError(makeError(HttpStatus.INTERNAL_SERVER_ERROR.value(), "Registered sessions returns as null"));

            HttpHeaders responseHeaders = new HttpHeaders();
            responseHeaders.setContentType(MediaType.APPLICATION_JSON);

            return new ResponseEntity<>(container, responseHeaders, HttpStatus.INTERNAL_SERVER_ERROR);
        }

        log.info("Registered session for peer [ip={}, port={}, name={}] is successful",
                activeSession.publicAddress.getHostName(),
                activeSession.publicAddress.getPort(),
                activeSession.peerName);

        ResponseContainer<String> container = new ResponseContainer<>();
        container.setBody("Successfully registered");

        HttpHeaders responseHeaders = new HttpHeaders();
        responseHeaders.setContentType(MediaType.APPLICATION_JSON);

        return new ResponseEntity<>(container, responseHeaders, HttpStatus.OK);
    }

    @GetMapping("/ping")
    public ResponseEntity<ResponseContainer> processPingRequest(@RequestParam("port") String natPort,
                                                                @RequestParam(value = "ip", required = false) String publicIp,
                                                                HttpServletRequest request)
    {
        if (publicIp == null || publicIp.isEmpty()){
            publicIp = request.getHeader("X-FORWARDED-FOR");
            if (publicIp == null || publicIp.isEmpty()){
                publicIp = request.getRemoteAddr();
            }
        }

        InetSocketAddress remoteUser = InetSocketAddress.createUnresolved(publicIp, Integer.parseInt(natPort));
        UdpSession activeSession = udpSessionsRepository.findByAddress(remoteUser);
        if (activeSession == null){
            return responseSessionExpiredOrNotExists();
        }
        activeSession.updateKeepAlive();

        /*log.debug("Update session for peer [ip={}, port={}, name={}]",
                activeSession.publicAddress.getHostName(),
                activeSession.publicAddress.getPort(),
                activeSession.peerName);*/

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
                                                                   @RequestParam("peerPort") String peerPort)
    {
        UdpSession session = udpSessionsRepository.findByPeerNameAndAddress(peerIp, Integer.parseInt(peerPort), peerName);
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
