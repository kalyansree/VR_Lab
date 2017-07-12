tcpip = StartServer();
[vertString, edgeString] = ReadFromUnity(tcpip);
fprintf('%s\n', vertString);
fprintf('%s\n', edgeString);
fclose(tcpip);

A = getNodeCoordArray(vertString);
B = getElementArray(edgeString);




