package com.zontwelg.rhms.com.zontwelg.rhms.domain;

import com.fasterxml.jackson.annotation.JsonIgnoreProperties;
import com.fasterxml.jackson.databind.annotation.JsonSerialize;

import java.util.ArrayList;
import java.util.Arrays;
import java.util.List;

@JsonSerialize
@JsonIgnoreProperties(ignoreUnknown = true)
public class ResponseContainer<T> {

    private T body;
    private List<Error> errors;

    public ResponseContainer() {
        errors = new ArrayList<Error>();
    }

    public ResponseContainer(Error... errors) {
        this.errors = Arrays.asList(errors);
    }

    public T getBody() {
        return body;
    }

    public void setBody(T body) {
        this.body = body;
    }

    public List<Error> getErrors() {
        return errors;
    }

    public void addError(Error error) {
        if (error != null) {
            this.errors.add(error);
        }
    }

    public void addAll(List<Error> errors) {
        this.errors.addAll(errors);
    }

    public boolean hasErrors() {
        return !this.errors.isEmpty();
    }

    public static class Error {
        private Integer code;

        private String message;
        public Error(){}

        public Error(Integer code) {
            this.code = code;
        }

        public Error(Integer code, String message) {
            this.code = code;
            this.message = message;
        }

        public Integer getCode() {
            return code;
        }

        public void setCode(Integer code) {
            this.code = code;
        }

        public String getMessage() {
            return message;
        }

        public void setMessage(String message) {
            this.message = message;
        }
    }
}
