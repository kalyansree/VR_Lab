function tcpipServer = StartServer()
    tcpipServer = tcpip('0.0.0.0',55000,'NetworkRole','Server');
    set(tcpipServer,'Timeout',10);
    fopen(tcpipServer);
end