//
//  ViewController.swift
//  rhms_client
//
//  Created by Vladislav Smetannikov on 26/04/2018.
//  Copyright © 2018 ZontWelg Team. All rights reserved.
//

import UIKit

class ViewController: UIViewController {

    override func viewDidLoad() {
        super.viewDidLoad()
        tryGetPeersList()
    }

    override func didReceiveMemoryWarning() {
        super.didReceiveMemoryWarning()
    }

    func tryGetPeersList(){
        let fullUrl = SettingsBundleHelper.getApiServerLink()
        let accessKey = SettingsBundleHelper.getApiAccessKey()
        
        let apiUrl = NSURL(string: "\(fullUrl)/udp-peer/list?accessKey=\(accessKey)")
        let request = NSMutableURLRequest(url: apiUrl! as URL, cachePolicy: .reloadIgnoringLocalAndRemoteCacheData, timeoutInterval: 10);
        
        let task = URLSession.shared.dataTask(with: request as URLRequest) {
            data, response, error in
            
            if (error != nil) {
                self.retryOrStopDiag(title: "API Request Error", message: "\(error?.localizedDescription ?? "Unknown")")
                print("error=\(String(describing: error))")
                return
            }
            
            DispatchQueue.main.async {
                if let data = data {
                    do {
                        let jsonDecoder = JSONDecoder()
                        let responseObj = try! jsonDecoder.decode(ApiSessionsList.self, from: data);
                        
                        if responseObj.errors != nil && !responseObj.errors.isEmpty {
                            let msgErrCode = responseObj.errors[0].code
                            let msgErrText = responseObj.errors[0].message
                            
                            self.retryOrStopDiag(title: "API Error: \(msgErrCode)", message: "\(msgErrText)")
                            print("API Error: \(responseObj.errors[0])")
                            return
                        }
                        
                        if responseObj.body == nil {
                            self.retryOrStopDiag(title: "API Error: -1", message: "Got empty response")
                            return
                        }
                    
                        let storyBoard: UIStoryboard = UIStoryboard(name: "Main", bundle: nil)
                        let newViewController = storyBoard.instantiateViewController(withIdentifier: "afterload")
                        
                        if let peerListController = newViewController as? PeerListViewController {
                            peerListController.preloadViewController = self
                            peerListController.preloadPeers(sessions: responseObj.body)
                        }
                        
                        self.navigationController!.pushViewController(newViewController, animated: true)
                    }
                } else if let error = error {
                    print(error.localizedDescription)
                }
            }
        }
        
        task.resume()
    }
    
    func retryOrStopDiag(title: String, message: String){
        let refreshAlert = UIAlertController(title: title, message: message, preferredStyle: UIAlertControllerStyle.alert)
        
        refreshAlert.addAction(UIAlertAction(title: "Retry update", style: .default, handler: { (action: UIAlertAction!) in
            print("Retrying get peer list ...")
            self.tryGetPeersList()
        }))
        
        refreshAlert.addAction(UIAlertAction(title: "Cancel", style: .cancel, handler: { (action: UIAlertAction!) in
            let storyBoard: UIStoryboard = UIStoryboard(name: "Main", bundle: nil)
            let newViewController = storyBoard.instantiateViewController(withIdentifier: "manualload")
            
            if let manualLoadViewController = newViewController as? ManualLoadViewController {
                manualLoadViewController.loadController = self
            }
            
            self.navigationController!.pushViewController(newViewController, animated: true)
            return
        }))
        
        self.present(refreshAlert, animated: true, completion: nil)
    }
    
    func failRefreshDiag(err: Error?){
        self.retryOrStopDiag(title: "API Update Error", message: "\(err?.localizedDescription ?? "Unknown")")
    }
}

