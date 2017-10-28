package com.zontwelg.rhms.controller;

import com.zontwelg.rhms.com.zontwelg.rhms.domain.ResponseContainer;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.boot.autoconfigure.web.ErrorAttributes;
import org.springframework.http.HttpHeaders;
import org.springframework.http.HttpStatus;
import org.springframework.http.MediaType;
import org.springframework.http.ResponseEntity;
import org.springframework.web.bind.annotation.CrossOrigin;
import org.springframework.web.bind.annotation.ExceptionHandler;
import org.springframework.web.bind.annotation.RequestMapping;
import org.springframework.web.bind.annotation.RestController;
import org.springframework.web.context.request.RequestAttributes;
import org.springframework.web.context.request.ServletRequestAttributes;

import javax.servlet.http.HttpServletRequest;
import javax.servlet.http.HttpServletResponse;
import java.util.Map;

@RestController
@CrossOrigin
public class ErrorController implements org.springframework.boot.autoconfigure.web.ErrorController {

    private static final Logger logger = LoggerFactory.getLogger(ErrorController.class);
    private static final String PATH = "/error";

    @Autowired
    private ErrorAttributes errorAttributes;

    @RequestMapping(value = PATH)
    public ResponseEntity<ResponseContainer> error(HttpServletRequest request, HttpServletResponse response) {
        ResponseContainer container = new ResponseContainer();
        RequestAttributes requestAttributes = new ServletRequestAttributes(request);
        HttpHeaders responseHeaders = new HttpHeaders();
        responseHeaders.setContentType(MediaType.APPLICATION_JSON);
        Map<String, Object> attributes = errorAttributes.getErrorAttributes(requestAttributes, false);
        container.addError(makeError(response.getStatus(), attributes.get("error").toString()));
        return new ResponseEntity<>(container, responseHeaders, HttpStatus.INTERNAL_SERVER_ERROR);
    }

    @ExceptionHandler(value = Exception.class)
    public ResponseEntity<ResponseContainer> handleInternalServerError(Exception e) {
        logger.error("Error: ", e);
        ResponseContainer container = new ResponseContainer();
        container.addError(makeError(HttpStatus.INTERNAL_SERVER_ERROR.value(), e.getMessage()));
        return ResponseEntity.status(HttpStatus.INTERNAL_SERVER_ERROR).body(container);
    }

    @Override
    public String getErrorPath() {
        return PATH;
    }

    private ResponseContainer.Error makeError(int code, String message) {
        return new ResponseContainer.Error(code, message);
    }
}
