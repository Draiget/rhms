package com.zontwelg.rhms.service;

import com.zontwelg.rhms.domain.UdpSession;

import java.net.InetSocketAddress;
import java.util.List;
import java.util.Optional;

public interface IUdpSessionsService {

    UdpSession registerSession(InetSocketAddress publicAddress, String peerName, String accessKey);
    UdpSession findByAddress(InetSocketAddress publicAddress);
    UdpSession findByPeerName(String peerName);
    UdpSession findByPeerNameAndAddress(String ip, int port, String peerName);
    List<UdpSession> findByAccessKey(String accessKey);
    boolean updatePeerStatus(InetSocketAddress publicAddress);
    boolean updatePeerName(InetSocketAddress publicAddress, String newPeerName);
    boolean removeSession(UdpSession session);
    boolean updateSpecificSession(UdpSession session, String newPeerName, String accessKey);
    boolean updateSpecificSession(UdpSession session, String newPeerName, String privatePort, String accessKey);
    Optional<UdpSession> findPeer(InetSocketAddress address);
    List<UdpSession> getAll();
}
