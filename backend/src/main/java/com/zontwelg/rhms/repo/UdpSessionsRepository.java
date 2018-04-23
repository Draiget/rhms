package com.zontwelg.rhms.repo;

import com.zontwelg.rhms.domain.UdpSession;
import org.slf4j.Logger;
import org.slf4j.LoggerFactory;
import org.springframework.scheduling.annotation.Scheduled;
import org.springframework.stereotype.Repository;

import java.util.ArrayList;
import java.util.Iterator;
import java.util.List;

@Repository
public class UdpSessionsRepository {

    private static final Logger log = LoggerFactory.getLogger("application");
    public List<UdpSession> sessionsList;

    public UdpSessionsRepository() {
        sessionsList = new ArrayList<>();
    }

    @Scheduled(fixedDelay = 5000)
    public void checkExpiredSessions(){
        Iterator it = sessionsList.iterator();
        while(it.hasNext()) {
            UdpSession session = (UdpSession) it.next();
            if (session.isTimedOut()){
                log.info("Removing client {} due to timeout", session);
                it.remove();
            }
        }
    }

}
