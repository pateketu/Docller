using System;

namespace Docller.Core.Common
{
    [Flags]
    [Serializable]
    public enum PermissionFlag
    {
        None = 0,
        DefaultFlag = 1,
        Read = 2,
        ReadWrite = 4,
        Admin = 8,
        CreateProject=16,
        //ShareAndTransmitFiles = 16,
        CreateProjects = 16,
        //Open = 64,
        //ControlPolicy = 64,
        //SerializationFormatter = 128,
        //ControlDomainPolicy = 256,
        //ControlPrincipal = 512,
        //ControlAppDomain = 1024,
        //RemotingConfiguration = 2048,
        //Infrastructure = 4096,
        //BindingRedirects = 8192,
        AllFlags =
            DefaultFlag | Read | ReadWrite | Admin | CreateProject
            //| InviteUsers | ShareAndTransmitFiles |
            //CreateProjects | Open
    }
}
