//
//  SettingsBundleHelper.swift
//  rhms_client
//
//  Created by Vladislav Smetannikov on 26/04/2018.
//  Copyright © 2018 ZontWelg Team. All rights reserved.
//

import Foundation
class SettingsBundleHelper {
    static let DefaultApiUrl = "http://srv-ps.rhms.zontwelg.com"
    static let DefaultApiAccessKey = "rhms-dc-zontwelg-shared"
    
    struct SettingsBundleKeys {
        static let ApiServer = "api_server"
        static let ApiAccessKey = "api_access_key"
    }
    
    class func getApiServerLink() -> String {
        let defaults = UserDefaults.standard
        
        if let apiLink = defaults.string(forKey: SettingsBundleKeys.ApiServer) {
            return apiLink;
        }
        
        defaults.set(DefaultApiUrl, forKey: SettingsBundleKeys.ApiServer);
        return DefaultApiUrl;
    }
    
    class func getApiAccessKey() -> String {
        let defaults = UserDefaults.standard
        
        if let apiLink = defaults.string(forKey: SettingsBundleKeys.ApiAccessKey) {
            return apiLink;
        }
        
        defaults.set(DefaultApiAccessKey, forKey: SettingsBundleKeys.ApiAccessKey);
        return DefaultApiAccessKey;
    }
}
