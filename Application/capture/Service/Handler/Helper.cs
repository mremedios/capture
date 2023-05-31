using System;
using Database.Models;
using SIPSorcery.SIP;

namespace Capture.Service.Handler;

public static class Helper
{
    public static SipMethods ToModel(this SIPMethodsEnum method)
    {
        switch (method)
        {
            case SIPMethodsEnum.OPTIONS: return SipMethods.OPTIONS;
            case SIPMethodsEnum.REGISTER: return SipMethods.REGISTER;
            case SIPMethodsEnum.MESSAGE: return SipMethods.MESSAGE;
            case SIPMethodsEnum.INVITE: return SipMethods.INVITE;
            case SIPMethodsEnum.ACK: return SipMethods.ACK;
            case SIPMethodsEnum.BYE: return SipMethods.BYE;
            case SIPMethodsEnum.INFO: return SipMethods.INFO;
            default: return Enum.Parse<SipMethods>(method.ToString());
        }
    }
}