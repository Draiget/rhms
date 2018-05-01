//
//  ManualLoadViewController.swift
//  rhms_client
//
//  Created by Vladislav Smetannikov on 30/04/2018.
//  Copyright © 2018 ZontWelg Team. All rights reserved.
//

import Foundation
import UIKit

class ManualLoadViewController : UIViewController {
    
    var loadController: ViewController!
    
    override func viewDidLoad() {
        self.navigationItem.hidesBackButton = true
        self.title = "API Error"
    }
    
    @IBAction func onRefreshButtonClick(_ sender: UIButton) {
        _ = navigationController?.popViewController(animated: true)
        loadController.tryGetPeersList()
    }
    
}
