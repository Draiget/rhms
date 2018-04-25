package com.zontwelg.rhms.service;

import com.zontwelg.rhms.domain.UdpSession;

import java.net.InetSocketAddress;
import java.util.List;
import java.util.Optional;

public interface IUdpSessionsService {

    UdpSession registerSession(InetSocketAddress publicAddress, String peerName);
    UdpSession findByAddress(InetSocketAddress publicAddress);
    UdpSession findByPeerName(String peerName);
    UdpSession findByPeerNameAndAddress(String ip, int port, String peerName);
    boolean updatePeerStatus(InetSocketAddress publicAddress);
    boolean updatePeerName(InetSocketAddress publicAddress, String newPeerName);
    boolean removeSession(UdpSession session);
    boolean updateSpecificSession(UdpSession session, String newPeerName);
    boolean updateSpecificSession(UdpSession session, String newPeerName, String privatePort);
    Optional<UdpSession> findPeer(InetSocketAddress address);
    List<UdpSession> getAll();
}
