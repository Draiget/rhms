package com.zontwelg.rhms.service;

import com.zontwelg.rhms.com.zontwelg.rhms.domain.ResponseContainer;

public class ErrorFactory {

    public static ResponseContainer.Error makeError(int code, String message) {
        return new ResponseContainer.Error(code, message);
    }

}
