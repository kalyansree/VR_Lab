function tcpipServer = StartServer()
    tcpipServer = tcpip('0.0.0.0',55000,'NetworkRole','Server');
    set(tcpipServer,'Timeout',5);
    fopen(tcpipServer);
end