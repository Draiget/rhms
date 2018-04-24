package com.zontwelg.rhms.domain;

import java.io.Serializable;
import java.net.InetAddress;
import java.net.InetSocketAddress;

public class UdpSession implements Serializable {

    public final long SESSION_TIMEOUT = 15000;

    private long lastAliveTime;
    public InetSocketAddress publicAddress;
    public String peerName;

    public UdpSession(){
        updateKeepAlive();
    }

    public UdpSession(InetSocketAddress address, String peerName){
        this();
        this.publicAddress = address;
        this.peerName = peerName;
    }

    public void updateKeepAlive() {
        lastAliveTime = System.currentTimeMillis();
    }

    public boolean isTimedOut() {
        return System.currentTimeMillis() - lastAliveTime > SESSION_TIMEOUT;
    }

    @Override
    public String toString() {
        return String.format("UdpSession[ip=%s, port=%d, name=%s]",
                publicAddress.getHostName(),
                publicAddress.getPort(),
                peerName);
    }
}
