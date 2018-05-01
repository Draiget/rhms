//
//  ResponseContainer.swift
//  rhms_client
//
//  Created by Vladislav Smetannikov on 26/04/2018.
//  Copyright © 2018 ZontWelg Team. All rights reserved.
//

import Foundation

struct ApiSessionsList : Codable {
    struct ApiError : Codable {
        var code : Int
        var message : String
    }
    
    struct ApiSession : Codable {
        var ipAddress : String
        var publicPort : UInt16
        var privatePort : UInt16
        var peerName : String
        var timedOut : Bool
    }
    
    var body : [ApiSession]!
    var errors : [ApiError]!
}
