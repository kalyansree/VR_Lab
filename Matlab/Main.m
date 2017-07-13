tcpip = StartServer();
[vertString, edgeString] = ReadFromUnity(tcpip);
fprintf('%s\n', vertString);
fprintf('%s\n', edgeString);
fclose(tcpip);

% vertString = '1(1.0000, 0.0000, 0.0000)I;2(0.0000, 1.0000, 1.0000)I;3(0.7000, 0.5500, 0.4000)O;4(1.0000, 0.5500, 0.4000)F;1(1.0000, 0.0000, 0.0000)T;1(1.0000, 0.0000, 0.0000)F;';
% edgeString = '1 3 3 2 3 4 ';

[node, I, O, F, T] = getNodeCoordArray(vertString);
elem = getElementArray(edgeString);



