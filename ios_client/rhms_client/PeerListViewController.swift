//
//  PeerListViewController.swift
//  rhms_client
//
//  Created by Vladislav Smetannikov on 26/04/2018.
//  Copyright © 2018 ZontWelg Team. All rights reserved.
//

import Foundation
import UIKit
import CocoaAsyncSocket

class PeerListViewController: UIViewController, UITableViewDataSource, UITableViewDelegate {

    @IBOutlet weak var PeersTableView: UITableView!
    
    var preloadViewController: ViewController!
    let sections = ["Available Peers"]
    var peerList : [ApiSessionsList.ApiSession] = []
    var updateTimer : Timer!
    
    override func viewDidLoad() {
        super.viewDidLoad()
        self.navigationItem.hidesBackButton = true
        self.PeersTableView.dataSource = self
        self.PeersTableView.delegate = self
        self.PeersTableView.addSubview(self.refreshControl)
        
        self.updateTimer = Timer.scheduledTimer(
            timeInterval: 60.0,
            target: self,
            selector: #selector(PeerListViewController.peersUpdateTimerTick),
            userInfo: nil,
            repeats: true)
    }
    
    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
    }
    
    func tableView(_ tableView: UITableView, titleForHeaderInSection section: Int) -> String? {
        return sections[section]
    }
    
    func numberOfSections(in tableView: UITableView) -> Int {
        return sections.count
    }
    
    func tableView(_ tableView: UITableView, cellForRowAt indexPath: IndexPath) -> UITableViewCell {
        let cell = tableView.dequeueReusableCell(withIdentifier: "peerCell", for: indexPath) as! PeerListCell
        cell.sessionData = peerList[indexPath.row]
        cell.applyFromSession(viewController: self)
        
        return cell
    }
    
    func tableView(_ tableView: UITableView, numberOfRowsInSection section: Int) -> Int {
        switch section {
            case 0:
                return peerList.count
            default:
                return 0
        }
    }
    
    @objc func peersUpdateTimerTick(){
        if !self.refreshControl.isRefreshing {
            self.refreshControl.beginRefreshing()
            updatePeerList()
        }
    }
    
    lazy var refreshControl: UIRefreshControl = {
        let refreshControl = UIRefreshControl()
        refreshControl.addTarget(self, action: #selector(PeerListViewController.handleRefresh(_:)), for: UIControlEvents.valueChanged)
        refreshControl.tintColor = UIColor.blue
        return refreshControl
    }()
    
    @objc func handleRefresh(_ refreshControl: UIRefreshControl) {
        updatePeerList()
    }
    
    func updatePeerList(){
        let fullUrl = SettingsBundleHelper.getApiServerLink()
        let accessKey = SettingsBundleHelper.getApiAccessKey()
        
        let apiUrl = NSURL(string: "\(fullUrl)/udp-peer/list?accessKey=\(accessKey)")
        let request = NSMutableURLRequest(url: apiUrl! as URL, cachePolicy: .reloadIgnoringLocalAndRemoteCacheData, timeoutInterval: 10);
        
        let task = URLSession.shared.dataTask(with: request as URLRequest) {
            data, response, error in
            
            // Check for error
            if (error != nil) {
                print("error=\(String(describing: error))")
                self.peerList = []
                
                DispatchQueue.main.async {
                    UIApplication.shared.registerForRemoteNotifications()
                    self.PeersTableView.reloadData()
                    self.refreshControl.endRefreshing()
                    
                    _ = self.navigationController?.popViewController(animated: true)
                    if self.preloadViewController != nil {
                        self.preloadViewController.failRefreshDiag(err: error)
                    }
                }
                
                return
            }
            
            DispatchQueue.main.async {
                UIApplication.shared.registerForRemoteNotifications()
                if let data = data {
                    do {
                        let jsonDecoder = JSONDecoder()
                        let responseObj = try! jsonDecoder.decode(ApiSessionsList.self, from: data);
                        
                        if !responseObj.errors.isEmpty {
                            print("API Error: \(responseObj.errors[0])")
                            self.peerList = []
                            self.PeersTableView.reloadData()
                            self.refreshControl.endRefreshing()
                            return
                        }
                        
                        self.preloadPeers(sessions: responseObj.body)
                        self.PeersTableView.reloadData()
                        self.refreshControl.endRefreshing()
                    }
                } else if let error = error {
                    print(error.localizedDescription)
                    self.peerList = []
                    self.PeersTableView.reloadData()
                    self.refreshControl.endRefreshing()
                }
            }
        }
        
        task.resume()
    }
    
    func preloadPeers(sessions: [ApiSessionsList.ApiSession]) {
        peerList = sessions
    }
}

class PeerListCell: UITableViewCell {
    
    var sessionData: ApiSessionsList.ApiSession!
    var tableViewController: PeerListViewController!
    var isSocketCreated: Bool = false
    var socket: OutSocket!
    
    let handshakeIn: [UInt8] = [ 0x92, 0x74, 0x53, 0x00 ]
    let handshakeOut: [UInt8] = [ 0x44, 0x43, 0x0020, 0x10, 0x32, 0x95 ]
    
    @IBOutlet weak var PeerNameLabel: UILabel!
    @IBOutlet weak var PeerIpLabel: UILabel!
    @IBOutlet weak var PeerConnectButton: UIButton!
    
    func beginHandshakeToPeer(){
        if self.socket == nil {
            return
        }
        
        let data = Data(buffer: UnsafeBufferPointer(start: handshakeOut, count: handshakeOut.count))
        
        showLoadingAlert(title: "Handshaking")
        
        self.socket.sendToData(data: data, recvFunc: {(data: Data!) -> Void in
            print("Receive data handler on Handshaking click")
            
            if data == nil {
                print("Sending packet seems to be timed out, data is nil")
                
                DispatchQueue.main.async {
                    self.tableViewController.dismiss(animated: true, completion: {() -> Void in
                        self.retryHandshakeOrCancel()
                    })
                }
                return
            }
            
            if data.count < 4 {
                print("Give invalid response from server, unknown lenght '\(data.count)'")
                
                DispatchQueue.main.async {
                    self.tableViewController.dismiss(animated: true, completion: nil)
                }
                return
            }
            
            if  data[0] == self.handshakeIn[0] &&
                data[1] == self.handshakeIn[1] &&
                data[2] == self.handshakeIn[2]
            {
                let magicByte: UInt8 = data[3]
                print("Valid response, magic server byte is \(magicByte)")
                
                DispatchQueue.main.async {
                    let storyBoard: UIStoryboard = UIStoryboard(name: "Main", bundle: nil)
                    let newViewController = storyBoard.instantiateViewController(withIdentifier: "peermanage")
                    
                    if let peerManageController = newViewController as? PeerManageViewController {
                        peerManageController.applyPeer(sock: self.socket, magicByte: magicByte)
                    }
                    
                    if self.tableViewController.navigationController == nil {
                        print("WARN: Navigation controller of PeerListViewController is nil!")
                    }
                    
                    self.tableViewController.dismiss(animated: true, completion: {
                        self.tableViewController.navigationController!.pushViewController(newViewController, animated: true)
                    })
                }
            }
        })
        
        /*var arr: [UInt32] = [32, 4, UInt32.max]
         let data = Data(buffer: UnsafeBufferPointer(start: &arr, count: arr.count))
         print(data)*/
    }
    
    @IBAction func onConnectClick(_ sender: UIButton) {
        beginHandshakeToPeer()
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
        
        self.tableViewController.present(pending, animated: true, completion: nil)
    }
    
    func retryHandshakeOrCancel(){
        let refreshAlert = UIAlertController(title: "Host Unreachable", message: "Host is not responding to handshaking (maybe host is timed out or connected over VPN)", preferredStyle: UIAlertControllerStyle.alert)
        
        refreshAlert.addAction(UIAlertAction(title: "Retry", style: .default, handler: { (action: UIAlertAction!) in
            self.beginHandshakeToPeer()
        }))
        
        refreshAlert.addAction(UIAlertAction(title: "Cancel", style: .cancel, handler: { (action: UIAlertAction!) in
            
        }))
        
        self.tableViewController.present(refreshAlert, animated: true, completion: nil)
    }
    
    func createSocket(){
        if !isSocketCreated {
            self.socket = OutSocket()
            isSocketCreated = true
        }
    }
    
    func applyFromSession(viewController: PeerListViewController){
        self.createSocket()
        
        self.tableViewController = viewController
        self.socket.applySession(session: self.sessionData)
        
        self.PeerIpLabel.text = "\(sessionData.ipAddress):\(sessionData.publicPort)"
        self.PeerNameLabel.text = sessionData.peerName
    }
}
