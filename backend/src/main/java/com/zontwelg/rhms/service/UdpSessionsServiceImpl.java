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

@Service
public class UdpSessionsServiceImpl implements IUdpSessionsService {

    @Autowired
    UdpSessionsRepository sessionsRepo;

    @Override
    public UdpSession registerSession(InetSocketAddress publicAddress, String peerName) {
        UdpSession old = findByAddress(publicAddress);
        if (old != null){
            old.peerName = peerName;
            old.updateKeepAlive();;
            return old;
        }

        UdpSession newSession = new UdpSession(publicAddress, peerName);
        sessionsRepo.sessionsList.add(newSession);
        return newSession;
    }

    @Override
    public UdpSession findByAddress(InetSocketAddress publicAddress) {
        return sessionsRepo.sessionsList
                .stream()
                .filter(x -> x.publicAddress.equals(publicAddress))
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
                .filter(x -> x.publicAddress.getHostName().equals(ip) && x.publicAddress.getPort() == port)
                .filter(x -> x.peerName.contentEquals(peerName))
                .findFirst()
                .orElse(null);
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
    public boolean updateSpecificSession(UdpSession session, String newPeerName) {
        if (!sessionsRepo.sessionsList.contains(session)){
            return false;
        }

        Optional<UdpSession> findSession = sessionsRepo.sessionsList.stream().filter(x -> x == session).findFirst();
        if (!findSession.isPresent()){
            return false;
        }

        UdpSession targetSession = findSession.get();
        targetSession.peerName = newPeerName;
        targetSession.updateKeepAlive();
        return true;
    }

    @Override
    public Optional<UdpSession> findPeer(InetSocketAddress address) {
        return sessionsRepo.sessionsList.stream().filter(x -> x.publicAddress.equals(address)).findFirst();
    }

    @Override
    public List<UdpSession> getAll() {
        return sessionsRepo.sessionsList;
    }
}
