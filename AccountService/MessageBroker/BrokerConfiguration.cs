﻿namespace AccountService.MessageBroker
{
    public class BrokerConfiguration
    {
        public string HostName { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Exchange { get; set; }
        public string Queue { get; set; }
    }
}
