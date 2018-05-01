//
//  PeerManageViewController.swift
//  rhms_client
//
//  Created by Vladislav Smetannikov on 01/05/2018.
//  Copyright © 2018 ZontWelg Team. All rights reserved.
//

import Foundation
import UIKit

class PeerManageViewController : UIViewController {
    
    @IBOutlet weak var peerIdLabel: UILabel!
    @IBOutlet weak var peerPingLabel: UILabel!
    @IBOutlet weak var loadIndicator: UIActivityIndicatorView!
    
    var peerInfo: ApiSessionsList.ApiSession!
    var socket: OutSocket!
    var commandSocket: OutSocket!
    var isObtainingPing: Bool = false
    var lastPingTime: Date!
    var updateTimer: Timer!
    var magicByte: UInt8!
    
    let handshakeIn: [UInt8] = [ 0x92, 0x74, 0x53, 0x00 ]
    let handshakeOut: [UInt8] = [ 0x44, 0x43, 0x0020, 0x10, 0x32, 0x95 ]
    
    override func viewDidLoad() {
        super.viewDidLoad()
        self.navigationController?.navigationBar.tintColor = UIColor.white
        
        self.loadIndicator.isHidden = false
        self.peerIdLabel.text = "\(self.peerInfo.peerName)";
        self.peerPingLabel.text = "Obtaining ping ..."
    }
    
    override func viewDidAppear(_ animated: Bool) {
        super.viewDidAppear(animated)
        self.updateTimer = Timer.scheduledTimer(
            timeInterval: 5,
            target: self,
            selector: #selector(PeerManageViewController.updatePeerTimerTick),
            userInfo: nil,
            repeats: true)
    }
    
    override func viewDidDisappear(_ animated: Bool) {
        super.viewDidDisappear(animated)
        if self.updateTimer != nil {
            self.updateTimer.invalidate()
            self.updateTimer = nil
        }
    }

    @objc func updatePeerTimerTick(){
        if self.peerInfo != nil {
            obtainPing()
        }
    }
    
    @IBAction func onRebootAction(_ sender: UIBarButtonItem) {
        self.sendRebootPacket()
    }
    
    func applyPeer(sock: OutSocket, magicByte: UInt8){
        self.peerInfo = sock.session
        self.magicByte = magicByte
        self.socket = sock
        self.isObtainingPing = false
        
        self.commandSocket = OutSocket()
        self.commandSocket.applySession(session: self.peerInfo)
    }
    
    func handshakeCmd(callback: ((UInt8?, String?)-> Void)! = nil){
        if self.commandSocket == nil {
            return
        }
        
        let data = Data(buffer: UnsafeBufferPointer(start: handshakeOut, count: handshakeOut.count))
        self.commandSocket.sendToData(data: data, recvFunc: {(data: Data!) -> Void in
            print("Receive data handler on Handshaking command socket")
            
            if data == nil {
                print("Command socket received handshake packet seems to be timed out, data is nil")
                if callback != nil {
                    callback(nil, "Received empty response from server")
                }
                return
            }
            
            if data.count < 4 {
                print("Command socket give invalid response from server when handshaking, unknown lenght '\(data.count)'")
                if callback != nil {
                    callback(nil, "Invalid response from server")
                }
                return
            }
            
            if  data[0] == self.handshakeIn[0] &&
                data[1] == self.handshakeIn[1] &&
                data[2] == self.handshakeIn[2]
            {
                let magicByte: UInt8 = data[3]
                print("Valid command socket handshake response, magic server byte is \(magicByte)")
                
                if callback != nil {
                    callback(magicByte, nil)
                }
                return
            }
            
            if callback != nil {
                callback(nil, "Invalid handshake padding received from server")
            }
        })
    }
    
    func sendRebootPacket(){
        self.showLoadingAlert(title: "Requesting to reboot")
        self.handshakeCmd( callback: {(cmdMagicByte: UInt8?, err: String?) -> Void in
            if cmdMagicByte == nil {
                DispatchQueue.main.async {
                    self.dismiss(animated: true, completion: {() -> Void in
                        self.alertUnable(title: "Request failed", message: "\(err ?? "Unknown error")")
                    })
                }
                return
            }
            
            var packetId: UInt32 = 2
            let resultData = NSMutableData()
            var cmbyte = cmdMagicByte
            
            resultData.append( NSData(bytes: &cmbyte, length: MemoryLayout<UInt8>.size) as Data )
            resultData.append( NSData(bytes: &packetId, length: MemoryLayout<UInt32>.size) as Data )
            
            DispatchQueue.main.async {
                self.dismiss(animated: true, completion: {() -> Void in
                   
                    self.commandSocket.sendToData(data: resultData as Data, recvFunc: { (d: Data!) in
                        print("Reboot packet got response")
                        if d == nil {
                            DispatchQueue.main.async {
                                self.dismiss(animated: true, completion: {() -> Void in
                                    self.alertUnable(title: "Request failed", message: "Got nullable response from peer")
                                })
                            }
                            
                            print("Empty reboot packet response, timeout")
                            return
                        }
                        
                        print("Reboot request send done, going back")
                        DispatchQueue.main.async {
                            self.navigationController!.popViewController(animated: true)
                        }
                    })
                    
                })
            }
        } )
    }
    
    func obtainPing(){
        if self.isObtainingPing {
            return
        }
        
        var packetId: UInt32 = 1
        let resultData = NSMutableData()
        resultData.append( NSData(bytes: &self.magicByte, length: MemoryLayout<UInt8>.size) as Data )
        resultData.append( NSData(bytes: &packetId, length: MemoryLayout<UInt32>.size) as Data )
        
        self.isObtainingPing = true
        self.loadIndicator.isHidden = false
        
        let startTime = Date()
        self.socket.sendToData(data: resultData as Data, recvFunc: { (d: Data!) in
            if d == nil {
                self.loadIndicator.isHidden = true
                self.peerPingLabel.text = "Ping: Timeout"
                
                self.retryLateOrGoBack(
                    title: "Ping timeout",
                    message: "Seems that remote peer is not responsing to our ping packets",
                    retryCallback: {()-> Void in
                        self.isObtainingPing = false
                })
                print("Empty ping response, timeout")
                return
            }
            
            let responseStr = String(describing: String(data: d, encoding: .utf8))
            print("Ping response: \(responseStr)")
            
            self.isObtainingPing = false
            self.lastPingTime = Date()
            let tookTime = self.lastPingTime.timeIntervalSince(startTime)
            print("Got response from ping packet from remote peer")
            print("Ping is \(tookTime) ms")
            
            DispatchQueue.main.async {
                self.loadIndicator.isHidden = true
                self.peerPingLabel.text = String(format: "Ping: %.3f ms.", tookTime)
            }
        })
    }
    
    func alertUnable(title: String, message: String, endCallback: (() -> Void)! = nil ){
        let refreshAlert = UIAlertController(title: title, message: message, preferredStyle: UIAlertControllerStyle.alert)
        
        refreshAlert.addAction(UIAlertAction(title: "Ok", style: .default, handler: { (action: UIAlertAction!) in
            if endCallback != nil {
                endCallback()
            }
        }))
        
        self.present(refreshAlert, animated: true, completion: nil)
    }
    
    func retryLateOrGoBack(title: String, message: String, retryCallback: (() -> Void)! = nil ){
        let refreshAlert = UIAlertController(title: title, message: message, preferredStyle: UIAlertControllerStyle.alert)
        
        refreshAlert.addAction(UIAlertAction(title: "Retry later", style: .default, handler: { (action: UIAlertAction!) in
            if retryCallback != nil {
                retryCallback()
            }
        }))
        
        refreshAlert.addAction(UIAlertAction(title: "Cancel", style: .cancel, handler: { (action: UIAlertAction!) in
            self.navigationController!.popViewController(animated: true)
            return
        }))
        
        self.present(refreshAlert, animated: true, completion: nil)
    }
    
    func showLoadingAlert(title: String){
        let pending = UIAlertController(title: title, message: nil, preferredStyle: .alert)
        
        let indicator = UIActivityIndicatorView()
        indicator.translatesAutoresizingMaskIntoConstraints = false
        indicator.color = #colorLiteral(red: 0.2196078449, green: 0.007843137719, blue: 0.8549019694, alpha: 1)
        pending.view.addSubview(indicator)
        
        pending.view.heightAnchor.constraint(equalToConstant: 100).isActive = true
        
        let views = ["pending" : pending.view, "indicator" : indicator]
        var constraints = NSLayoutConstraint.constraints(withVisualFormat: "V:[indicator]-(20)-|", options: .alignAllTop, metrics: nil, views: views)
        constraints += NSLayoutConstraint.constraints(withVisualFormat: "H:|[indicator]|", options: .alignAllFirstBaseline, metrics: nil, views: views)
        pending.view.addConstraints(constraints)
        pending.view.sizeToFit()
        
        indicator.isUserInteractionEnabled = false
        indicator.startAnimating()
        
        self.present(pending, animated: true, completion: nil)
    }
}
