package com.zontwelg.rhms.controller;

import com.zontwelg.rhms.com.zontwelg.rhms.domain.ResponseContainer;
import com.zontwelg.rhms.service.ErrorFactory;

public abstract class AbstractController {

    private ErrorFactory errorFactory = new ErrorFactory();

    protected ResponseContainer.Error makeError(int code, String message) {
        return errorFactory.makeError(code, message);
    }

}
