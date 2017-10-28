package com.zontwelg.rhms.controller;

import com.zontwelg.rhms.com.zontwelg.rhms.domain.ResponseContainer;
import com.zontwelg.rhms.com.zontwelg.rhms.domain.UdpSession;
import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.stereotype.Controller;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RequestMethod;
import org.springframework.web.bind.annotation.RequestParam;
import org.springframework.web.bind.annotation.ResponseBody;

@Controller
@RequestMapping("/udp-hp")
public class UdpPunchingController extends AbstractController {

    @RequestMapping(value = "/make-session", method = RequestMethod.GET)
    public ResponseEntity<ResponseContainer<UdpSession>> processSessionRequest(){
        ResponseContainer<UdpSession> container = new ResponseContainer<>();
        container.addError(makeError(HttpStatus.BAD_REQUEST.value(), "Not implemented"));

        HttpHeaders responseHeaders = new HttpHeaders();
        responseHeaders.setContentType(MediaType.APPLICATION_JSON);

        return new ResponseEntity<>(container, responseHeaders, HttpStatus.OK);
    }

    @RequestMapping(value = "/test", method = RequestMethod.GET)
    public ResponseEntity<ResponseContainer<UdpSession>> test(){
        ResponseContainer<UdpSession> container = new ResponseContainer<>();

        HttpHeaders responseHeaders = new HttpHeaders();
        responseHeaders.setContentType(MediaType.APPLICATION_JSON);

        return new ResponseEntity<>(container, responseHeaders, HttpStatus.OK);
    }

}
