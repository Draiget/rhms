//
//  UDPServerConnection.swift
//  rhms_client
//
//  Created by Vladislav Smetannikov on 26/04/2018.
//  Copyright © 2018 ZontWelg Team. All rights reserved.
//

import Foundation
import CocoaAsyncSocket

class OutSocket: NSObject, GCDAsyncUdpSocketDelegate {

    var socket: GCDAsyncUdpSocket!
    var session: ApiSessionsList.ApiSession!
    var onReceive: ((Data?) -> Void)!
    var recvTimeoutTimer: Timer!

    override init(){
        super.init()
        socket = GCDAsyncUdpSocket(delegate: self, delegateQueue: DispatchQueue.main)
    }
    
    func applySession(session: ApiSessionsList.ApiSession){
        self.session = session
    }

    func send(message:String){
        let data = message.data(using: .utf8)!
        socket.send(data, withTimeout: 2, tag: 0)
    }
    
    func sendTo(msg: String){
        self.sendToData(data: msg.data(using: .utf8)!)
    }
    
    func sendToData(data: Data, recvFunc: ((Data?) -> Void)! = nil){
        self.onReceive = recvFunc
        socket.send(data, toHost: self.session.ipAddress, port: self.session.publicPort, withTimeout: 10.0, tag: 0)
        try! socket.receiveOnce()
        
        self.recvTimeoutTimer = Timer.scheduledTimer(
            timeInterval: 10.0,
            target: self,
            selector: #selector(OutSocket.onSendFailTimer),
            userInfo: nil,
            repeats: true)
    }
    
    @objc func onSendFailTimer(){
        if self.recvTimeoutTimer != nil {
            self.recvTimeoutTimer.invalidate()
            self.recvTimeoutTimer = nil
        }
        
        if self.onReceive != nil {
            print("Calling packet receive timeout!")
            self.onReceive(nil)
            self.onReceive = nil
            return
        }
    }
    
    func udpSocket(_ sock: GCDAsyncUdpSocket, didReceive data: Data, fromAddress address: Data, withFilterContext filterContext: Any?) {
        if self.recvTimeoutTimer != nil {
            self.recvTimeoutTimer.invalidate()
            self.recvTimeoutTimer = nil
        }
        
        let addrArr = address.withUnsafeBytes {
            [UInt8](UnsafeBufferPointer(start: $0, count: address.count))
        }
        
        print("Receive packet in socket from: \(addrArr[0]).\(addrArr[1]).\(addrArr[2]).\(addrArr[3])")
        /*print("==== recv ====")
        print("data: \(data) -> \(String(describing: String(data: data, encoding: .utf8)))")
        print("address: \(address) -> \(String(describing: String(data: address, encoding: .utf8))) -> \(address as NSData)")
        print("context: \(String(describing: filterContext))")
        print("==============")*/
        
        if self.onReceive != nil {
            self.onReceive(data)
            self.onReceive = nil
        }
    }

    func udpSocket(_ sock: GCDAsyncUdpSocket, didSendDataWithTag tag: Int) {
        print("didSendDataWithTag")
    }
    
    func udpSocket(_ sock: GCDAsyncUdpSocket, didNotConnect error: Error?) {
        print("didNotConnect")
    }

    func udpSocket(_ sock: GCDAsyncUdpSocket, didNotSendDataWithTag tag: Int, dueToError error: Error?) {
        print("didNotSendDataWithTag")
    }
}
