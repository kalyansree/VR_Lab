tcpip = StartServer();
[vertString, edgeString] = ReadFromUnity(tcpip);
fprintf('%s\n', vertString);
fprintf('%s\n', edgeString);
fclose(tcpip);

%Example vertString%
%1(1.0000, 0.0000, 0.0000)I 2(0.0000, 1.0000, 1.0000)O 3(0.7000, 0.5500,
%0.4000)T 4(1.0000, 0.5500, 0.4000)F %

%Example edgeString%
%1 3 3 2 3 4 %

