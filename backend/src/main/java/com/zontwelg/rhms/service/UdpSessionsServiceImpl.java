package com.zontwelg.rhms.service;

import com.zontwelg.rhms.domain.UdpSession;
import com.zontwelg.rhms.repo.UdpSessionsRepository;
import org.springframework.beans.factory.annotation.Autowired;
import org.springframework.context.annotation.Bean;
import org.springframework.stereotype.Service;

import java.net.InetSocketAddress;
import java.util.ArrayList;
import java.util.List;
import java.util.Optional;
import java.util.stream.Collectors;

@Service
public class UdpSessionsServiceImpl implements IUdpSessionsService {

    @Autowired
    UdpSessionsRepository sessionsRepo;

    @Override
    public UdpSession registerSession(InetSocketAddress publicAddress, String peerName, String accessKey) {
        UdpSession old = findByAddress(publicAddress);
        if (old != null){
            old.peerName = peerName;
            old.updateKeepAlive();;
            return old;
        }

        UdpSession newSession = new UdpSession(publicAddress, peerName, accessKey);
        sessionsRepo.sessionsList.add(newSession);
        return newSession;
    }

    @Override
    public UdpSession findByAddress(InetSocketAddress publicAddress) {
        return sessionsRepo.sessionsList
                .stream()
                .filter(x -> x.getPublicAddress().equals(publicAddress))
                .findFirst()
                .orElse(null);
    }

    @Override
    public UdpSession findByPeerName(String peerName) {
        return sessionsRepo.sessionsList
                .stream()
                .filter(x -> x.peerName.contentEquals(peerName))
                .findFirst()
                .orElse(null);
    }

    @Override
    public UdpSession findByPeerNameAndAddress(String ip, int port, String peerName) {
        return sessionsRepo.sessionsList
                .stream()
                .filter(x -> x.getPublicAddress().getHostName().equals(ip) && x.getPublicAddress().getPort() == port)
                .filter(x -> x.peerName.contentEquals(peerName))
                .findFirst()
                .orElse(null);
    }

    @Override
    public List<UdpSession> findByAccessKey(String accessKey) {
        return sessionsRepo.sessionsList
                .stream()
                .filter(x -> x.getAccessKey().contentEquals(accessKey))
                .collect(Collectors.toList());
    }

    @Override
    public boolean updatePeerStatus(InetSocketAddress publicAddress) {
        Optional<UdpSession> findSession = findPeer(publicAddress);
        if (!findSession.isPresent()){
            return false;
        }

        UdpSession targetSession = findSession.get();
        targetSession.updateKeepAlive();
        return true;
    }

    @Override
    public boolean updatePeerName(InetSocketAddress publicAddress, String newPeerName) {
        Optional<UdpSession> findSession = findPeer(publicAddress);
        if (!findSession.isPresent()){
            return false;
        }

        UdpSession targetSession = findSession.get();
        targetSession.peerName = newPeerName;
        return true;
    }

    @Override
    public boolean removeSession(UdpSession session) {
        return sessionsRepo.sessionsList.remove(session);
    }

    @Override
    public boolean updateSpecificSession(UdpSession session, String newPeerName, String accessKey) {
        return updateSpecificSession(session, newPeerName, null);
    }

    @Override
    public boolean updateSpecificSession(UdpSession session, String newPeerName, String privatePort, String accessKey) {
        if (!sessionsRepo.sessionsList.contains(session)){
            return false;
        }

        Optional<UdpSession> findSession = sessionsRepo.sessionsList.stream().filter(x -> x == session).findFirst();
        if (!findSession.isPresent()){
            return false;
        }

        UdpSession targetSession = findSession.get();
        targetSession.peerName = newPeerName;

        if (privatePort != null && !privatePort.isEmpty()) {
            try {
                targetSession.privatePort = Integer.parseInt(privatePort);
            } catch (NumberFormatException e) {
                targetSession.privatePort = 0;
            }
        }

        targetSession.updateKeepAlive();
        return true;
    }

    @Override
    public Optional<UdpSession> findPeer(InetSocketAddress address) {
        return sessionsRepo.sessionsList.stream().filter(x -> x.getPublicAddress().equals(address)).findFirst();
    }

    @Override
    public List<UdpSession> getAll() {
        return sessionsRepo.sessionsList;
    }
}
