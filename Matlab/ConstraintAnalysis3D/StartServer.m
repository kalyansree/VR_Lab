function tcpipServer = StartServer()
    tcpipServer = tcpip('0.0.0.0',55000,'NetworkRole','Server');
    set(tcpipServer,'Timeout',10);
    tcpipServer.OutputBufferSize = 10000;
    fopen(tcpipServer);
end