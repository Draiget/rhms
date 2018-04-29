package com.zontwelg.rhms.domain;

import com.fasterxml.jackson.annotation.JsonIgnore;

import java.io.Serializable;
import java.net.InetAddress;
import java.net.InetSocketAddress;

public class UdpSession implements Serializable {

    public final long SESSION_TIMEOUT = 15000;

    private long lastAliveTime;

    @JsonIgnore
    private InetSocketAddress publicAddress;

    @JsonIgnore
    private String accessKey;

    public String ipAddress;
    public int publicPort;
    public int privatePort;
    public String peerName;

    public UdpSession(){
        updateKeepAlive();
    }

    public UdpSession(InetSocketAddress address, String peerName, String accessKey){
        this();
        this.accessKey = accessKey;
        this.publicAddress = address;
        this.peerName = peerName;
        this.publicPort = address.getPort();
        this.ipAddress = address.getHostName();
    }

    public void updateKeepAlive() {
        lastAliveTime = System.currentTimeMillis();
    }

    public boolean isTimedOut() {
        return System.currentTimeMillis() - lastAliveTime > SESSION_TIMEOUT;
    }

    public InetSocketAddress getPublicAddress() {
        return publicAddress;
    }

    public String getAccessKey() {
        return accessKey;
    }

    @Override
    public String toString() {
        return String.format("UdpSession[ip=%s, port=%d, name=%s]",
                publicAddress.getHostName(),
                publicAddress.getPort(),
                peerName);
    }
}
