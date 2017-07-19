tcpip = StartServer();
[vertString, edgeString, forceString] = ReadFromUnity(tcpip);
fprintf('%s\n', vertString);
fprintf('%s\n', edgeString);
fprintf('%s\n', forceString);


% vertString = '1(1.0000, 0.0000, 0.0000)I;2(0.0000, 1.0000, 1.0000)I;3(0.7000, 0.5500, 0.4000)O;4(1.0000, 0.5500, 0.4000)F;5(1.0000, 0.0000, 0.0000)T;6(1.0000, 0.0000, 0.0000)F;';
% edgeString = '1 3 3 2 3 4 ';
% forceString = '4(1.0000, 0.0000, 0.0000)0.8;2(0.0000, 1.0000, 1.0000)1%

[node, I, O, F, T] = getNodeCoordArray(vertString);
elem = getElementArray(edgeString);

%[ u ] = mat_unity( node, I, O, F, T, elem);%
node(:,2:size(node,2)) = node(:,2:size(node,2)) + 0.1
retString = getDeformedCoordString(node);
fwrite(tcpip,retString);
fwrite(tcpip,'|');
fwrite(tcpip, edgeString);
fclose(tcpip);

