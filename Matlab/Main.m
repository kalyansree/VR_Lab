tcpip = StartServer();
[vertString, edgeString, forceString] = ReadFromUnity(tcpip);
fprintf('%s\n', vertString);
fprintf('%s\n', edgeString);
fprintf('%s\n', forceString);


%vertString = '1(1.0000, 0.8500, 0.0000)I;2(1.0000, 1.0000, 0.0000)F;3(0.5000, 0.2000, 0.5000)T;4(0.5000, 0.8500, 1.0000)O;5(0.5000, 0.5000, 1.0000)F;6(0.5000, 0.2000, 0.0000)F;';
%edgeString = '1 2 1 3 4 5 4 3 3 6 ';
%forceString = '1(1.0000, 0.0000, 0.0000);';
nmesh = 10;
[node, I, O, F, T] = getNodeCoordArray(vertString);
nnode = size(node,1);
elem = getElementArray(edgeString, nmesh);
force = getForceArray(forceString);
disp_scaling = 1;
[ u ] = mat_unity( node, I, O, F, T, elem, force, disp_scaling);
%u = u * disp_scaling + node;
%Can comment out the line below once the returned u is working correctly%
%Basically this is just testing code%
%node(:,2:size(node,2)) = node(:,2:size(node,2)) + 0.1;%

%Again, use u here instead of node once everything is working%
retString = getDeformedCoordString(u, nnode);
edgeString = getDeformedEdgeString(edgeString, nnode, nmesh); 
fwrite(tcpip,retString);
fwrite(tcpip,'|');
fwrite(tcpip, edgeString);
fclose(tcpip);

