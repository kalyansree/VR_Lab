function [Cglobal,Felem1]=LoadFlow_general(NNODE,NELEM,Kglobal,BC,R,dim)
%Summary of this function goes here
% Outputs : 
% Cglobal = glabal complaince matrix 
% Felem = elemental (transferred) forces
% Inputs :
% nNode = no. of nodes
% nElem = no. of elements
% nPoint = no. of points at which bc's are defined 
% Kglobal = global stiffness matrix 
%   Detailed explanation goes here


%nodeSet = nodeSet(:);
% nNode=size(nodeSet,1);%length(nodeSet);
% nElem=size(elemSet,1);
nNode = NNODE;
nELEM = NELEM; 
% nPoint=size(obj.BC,1);%length(obj.BC(:,1));
% BC=nan(nNode,6);BC(1:nPoint,:)=obj.BC;
BC=BC(:);
% Kglobal = getGMatrix(obj,'stiff',nodeSet,elemSet,matSet,sectSet);

K = Kglobal;
BC2 = BC;
% this program converts all the non-zero displacements as a
% free bc's
for i = 1:numel(BC)
    if BC(i) ~= 0
        BC2(i) = nan;
    end
end



freeBC=isnan(BC2);
Kreduced=K(freeBC,freeBC);
Creduced=inv(Kreduced);

if dim == 2
    Cglobal=zeros(3*nNode,3*nNode);
    Cglobal(freeBC,freeBC)=Creduced;
    
    Felem = 0*ones(3,nNode);    
    for i=1:nNode,
        %i*3-2 x 
        %i*3-1 y
        %i*3   z
        BC3 = freeBC(i*3-2:i*3);
        num = sum(BC3 == 1);
        tempvar = zeros(num,1);
        if BC3 == zeros(3,1)
            Felem(1:3,i) = NaN*ones(3,1);
        elseif BC3 == [1;0;0]
            Cii = Cglobal(i*3-2,i*3-2);
            for j = 1:nNode
                Cij = Cglobal(i*3-2,j*3-2);
                R1 = R(j*3-2);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem(1,i) = (Cii)\tempvar;
        elseif BC3 == [0;1;0]
            Cii = Cglobal(i*3-1,i*3-1);
            for j = 1:nNode
                Cij = Cglobal(i*3-1,j*3-1);
                R1 = R(j*3-1);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem(2,i) = (Cii)\tempvar;
        elseif BC3 == [0;0;1]
            Cii = Cglobal(i*3,i*3);
            for j = 1:nNode
                Cij = Cglobal(i*3,j*3);
                R1 = R(j*3);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem(3,i) = (Cii)\tempvar;
        elseif BC3 == [1;1;0]
            Cii = Cglobal([i*3-2 i*3-1],[i*3-2 i*3-1]);
            for j = 1:nNode
                Cij = Cglobal([i*3-2 i*3-1],[j*3-2 j*3-1]);
                R1 = R([j*3-2 j*3-1]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1,2],i) = (Cii)\tempvar;
            Felem(3,i) = 0;
        elseif BC3 == [1;0;1]
            Cii = Cglobal([i*3-2 i*3],[i*3-2 i*3]);
            for j = 1:nNode
                Cij = Cglobal([i*3-2 i*3],[j*3-2 j*3]);
                R1 = R([j*3-2 j*3]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1,3],i) = (Cii)\tempvar;
            Felem(2,i) = 0;
        elseif BC3 == [0;1;1]
            Cii = Cglobal([i*3-1 i*3],[i*3-1 i*3]);
            for j = 1:nNode
                Cij = Cglobal([i*3-1 i*3],[j*3-1 j*3]);
                R1 = R([j*3-1 j*3]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([2,3],i) = (Cii)\tempvar;
            Felem(1,i) = 0;
        elseif BC3 == [1;1;1]
            Cii = Cglobal([i*3-2 i*3-1 i*3],[i*3-2 i*3-1 i*3]);
            for j = 1:nNode
                Cij = Cglobal([i*3-2 i*3-1 i*3],[j*3-2 j*3-1 j*3]);
                R1 = R([j*3-2 j*3-1 j*3]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem(1:3,i) = (Cii)\tempvar;
        end
    end
                         
elseif dim == 3
    Cglobal=zeros(6*nNode,6*nNode);
    Cglobal(freeBC,freeBC)=Creduced;
    
    Felem = 0*ones(6,nNode);
    
    for i=1:nNode,
        %i*6-5 x
        %i*6-4 y
        %i*6-3 z
        %i*6-2 xt
        %i*6-1 yt
        %i*6   zt
        BC3 = freeBC(i*6-5:i*6);
        num = sum(BC3 == 1);
        tempvar = zeros(num,1);
        if BC3 == zeros(6,1) % case 1 all zero
            Felem(1:6,i) = NaN*ones(6,1);
        elseif BC3 == [0;0;0;0;0;1] % case 2 only zt
            Cii = Cglobal(i*6,i*6);
            for j = 1:nNode
                Cij = Cglobal(i*6,j*6);
                R1 = R(j*6);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem(6,i) = (Cii)\tempvar;
        elseif BC3 == [0;0;0;0;1;0] % case 3 only yt
            Cii = Cglobal(i*6-1,i*6-1);
            for j = 1:nNode
                Cij = Cglobal(i*6-1,j*6-1);
                R1 = R(j*6-1);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem(5,i) = (Cii)\tempvar;
        elseif BC3 == [0;0;0;0;1;1] % case 4 yt zt
            Cii = Cglobal([i*6-1 i*6],[i*6-1 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-1 i*6],[j*6-1 j*6]);
                R1 = R([j*6-1 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([5 6],i) = (Cii)\tempvar;
        elseif BC3 == [0;0;0;1;0;0] % case 5 only xt
            Cii = Cglobal(i*6-2,i*6-2);
            for j = 1:nNode
                Cij = Cglobal(i*6-2,j*6-2);
                R1 = R(j*6-2);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem(4,i) = (Cii)\tempvar;
        elseif BC3 == [0;0;0;1;0;1] % case 6 xt zt
            Cii = Cglobal([i*6-2 i*6],[i*6-2 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-2 i*6],[j*6-2 j*6]);
                R1 = R([j*6-2 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([4 6],i) = (Cii)\tempvar;
        elseif BC3 == [0;0;0;1;0;1] % case 7 xt yt
            Cii = Cglobal([i*6-2 i*6-1],[i*6-2 i*6-1]);
            for j = 1:nNode
                Cij = Cglobal([i*6-2 i*6-1],[j*6-2 j*6-1]);
                R1 = R([j*6-2 j*6-1]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([4 5],i) = (Cii)\tempvar;
        elseif BC3 == [0;0;0;1;1;1] % case 8 xt yt zt
            Cii = Cglobal([i*6-2 i*6-1 i*6],[i*6-2 i*6-1 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-2 i*6-1 i*6],[j*6-2 j*6-1 j*6]);
                R1 = R([j*6-2 j*6-1 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([4 5 6],i) = (Cii)\tempvar;
        elseif BC3 == [0;0;1;0;0;0] % case 9  only z
            Cii = Cglobal(i*6-3,i*6-3);
            for j = 1:nNode
                Cij = Cglobal(i*6-3,j*6-3);
                R1 = R(j*6-3);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem(3,i) = (Cii)\tempvar;
        elseif BC3 == [0;0;1;0;0;1] % case 10 z zt
            Cii = Cglobal([i*6-3 i*6],[i*6-3 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-3 i*6],[j*6-3 j*6]);
                R1 = R([j*6-3 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([3 6],i) = (Cii)\tempvar;
        elseif BC3 == [0;0;1;0;1;0] % case 11 z yt
            Cii = Cglobal([i*6-3 i*6-1],[i*6-3 i*6-1]);
            for j = 1:nNode
                Cij = Cglobal([i*6-3 i*6-1],[j*6-3 j*6-1]);
                R1 = R([j*6-3 j*6-1]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([3 5],i) = (Cii)\tempvar;
        elseif BC3 == [0;0;0;0;1;1] % case 12 z yt zt
            Cii = Cglobal([i*6-3 i*6-1 i*6],[i*6-3 i*6-1 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-3 i*6-1 i*6],[j*6-3 j*6-1 j*6]);
                R1 = R([j*6-3 j*6-1 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([3 5 6],i) = (Cii)\tempvar;
        elseif BC3 == [0;0;1;1;0;0] % case 13 z xt
            Cii = Cglobal([i*6-3 i*6-2],[i*6-3 i*6-2]);
            for j = 1:nNode
                Cij = Cglobal([i*6-3 i*6-2],[j*6-3 j*6-2]);
                R1 = R([j*6-3 j*6-2]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([3 4],i) = (Cii)\tempvar;
        elseif BC3 == [0;0;0;1;0;1] % case 14 z xt zt
            Cii = Cglobal([i*6-3 i*6-2 i*6],[i*6-3 i*6-2 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-3 i*6-2 i*6],[j*6-3 j*6-2 j*6]);
                R1 = R([j*6-3 j*6-2 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([3 4 6],i) = (Cii)\tempvar;
        elseif BC3 == [0;0;1;1;0;1] % case 15 z xt yt
            Cii = Cglobal([i*6-3 i*6-2 i*6-1],[i*6-3 i*6-2 i*6-1]);
            for j = 1:nNode
                Cij = Cglobal([i*6-3 i*6-2 i*6-1],[j*6-3 j*6-2 j*6-1]);
                R1 = R([j*6-3 j*6-2 j*6-1]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([3 4 5],i) = (Cii)\tempvar;
        elseif BC3 == [0;0;1;1;1;1] % case 16 z xt yt zt
            Cii = Cglobal([i*6-3 i*6-2 i*6-1 i*6],[i*6-3 i*6-2 i*6-1 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-3 i*6-2 i*6-1 i*6],[j*6-3 j*6-2 j*6-1 j*6]);
                R1 = R([j*6-3 j*6-2 j*6-1 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([3 4 5 6],i) = (Cii)\tempvar;
        elseif BC3 == [0;1;0;0;0;0] % case 17 only y
            Cii = Cglobal(i*6-4,i*6-4);
            for j = 1:nNode
                Cij = Cglobal(i*6-4,j*6-4);
                R1 = R(j*6-4);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem(2,i) = (Cii)\tempvar;
        elseif BC3 == [0;1;0;0;0;1] % case 18 y zt
            Cii = Cglobal([i*6-4 i*6],[i*6-4 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-4 i*6],[j*6-4 j*6]);
                R1 = R([j*6-4 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([2 6],i) = (Cii)\tempvar;
        elseif BC3 == [0;1;0;0;1;0] % case 19 y yt
            Cii = Cglobal([i*6-4 i*6-1],[i*6-4 i*6-1]);
            for j = 1:nNode
                Cij = Cglobal([i*6-4 i*6-1],[j*6-4 j*6-1]);
                R1 = R([j*6-4 j*6-1]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([2 5],i) = (Cii)\tempvar;
        elseif BC3 == [0;1;0;0;1;1] % case 20 y yt zt
            Cii = Cglobal([i*6-4 i*6-1 i*6],[i*6-4 i*6-1 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-4 i*6-1 i*6],[j*6-4 j*6-1 j*6]);
                R1 = R([j*6-4 j*6-1 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([2 5 6],i) = (Cii)\tempvar;
        elseif BC3 == [0;1;0;1;0;0] % case 21 y xt
            Cii = Cglobal([i*6-4 i*6-2],[i*6-4 i*6-2]);
            for j = 1:nNode
                Cij = Cglobal([i*6-4 i*6-2],[j*6-4 j*6-2]);
                R1 = R([j*6-4 j*6-2]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([2 4],i) = (Cii)\tempvar;
        elseif BC3 == [0;1;0;1;0;1] % case 22 y xt zt
            Cii = Cglobal([i*6-4 i*6-2 i*6],[i*6-4 i*6-2 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-4 i*6-2 i*6],[j*6-4 j*6-2 j*6]);
                R1 = R([j*6-4 j*6-2 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([2 4 6],i) = (Cii)\tempvar;
        elseif BC3 == [0;1;0;1;0;1] % case 23 y xt yt
            Cii = Cglobal([i*6-4 i*6-2 i*6-1],[i*6-4 i*6-2 i*6-1]);
            for j = 1:nNode
                Cij = Cglobal([i*6-4 i*6-2 i*6-1],[j*6-4 j*6-2 j*6-1]);
                R1 = R([j*6-4 j*6-2 j*6-1]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([2 4 5],i) = (Cii)\tempvar;
        elseif BC3 == [0;1;0;1;1;1] % case 24 y xt yt zt
            Cii = Cglobal([i*6-4 i*6-2 i*6-1 i*6],[i*6-4 i*6-2 i*6-1 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-4 i*6-2 i*6-1 i*6],[j*6-4 j*6-2 j*6-1 j*6]);
                R1 = R([j*6-4 j*6-2 j*6-1 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([2 4 5 6],i) = (Cii)\tempvar;
        elseif BC3 == [0;1;1;0;0;0] % case 25 y z
            Cii = Cglobal([i*6-4 i*6-3],[i*6-4 i*6-3]);
            for j = 1:nNode
                Cij = Cglobal([i*6-4 i*6-3],[j*6-4 j*6-3]);
                R1 = R([j*6-4 j*6-3]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([2 3],i) = (Cii)\tempvar;
        elseif BC3 == [0;1;1;0;0;1] % case 26 y z zt
            Cii = Cglobal([i*6-4 i*6-3 i*6],[i*6-4 i*6-3 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-4 i*6-3 i*6],[j*6-4 j*6-3 j*6]);
                R1 = R([j*6-4 j*6-3 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([2 3 6],i) = (Cii)\tempvar;
        elseif BC3 == [0;1;1;0;1;0] % case 27 y z yt
            Cii = Cglobal([i*6-4 i*6-3 i*6-1],[i*6-4 i*6-3 i*6-1]);
            for j = 1:nNode
                Cij = Cglobal([i*6-4 i*6-3 i*6-1],[j*6-4 j*6-3 j*6-1]);
                R1 = R([j*6-4 j*6-3 j*6-1]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([2 3 5],i) = (Cii)\tempvar;
        elseif BC3 == [0;1;0;0;1;1] % case 28 y z yt zt
            Cii = Cglobal([i*6-4 i*6-3 i*6-1 i*6],[i*6-4 i*6-3 i*6-1 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-4 i*6-3 i*6-1 i*6],[j*6-4 j*6-3 j*6-1 j*6]);
                R1 = R([j*6-4 j*6-3 j*6-1 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([2 3 5 6],i) = (Cii)\tempvar;
        elseif BC3 == [0;1;1;1;0;0] % case 29 y z xt
            Cii = Cglobal([i*6-4 i*6-3 i*6-2],[i*6-4 i*6-3 i*6-2]);
            for j = 1:nNode
                Cij = Cglobal([i*6-4 i*6-3 i*6-2],[j*6-4 j*6-3 j*6-2]);
                R1 = R([j*6-4 j*6-3 j*6-2]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([2 3 4],i) = (Cii)\tempvar;
        elseif BC3 == [0;0;0;1;0;1] % case 30 y z xt zt
            Cii = Cglobal([i*6-4 i*6-3 i*6-2 i*6],[i*6-4 i*6-3 i*6-2 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-4 i*6-3 i*6-2 i*6],[j*6-4 j*6-3 j*6-2 j*6]);
                R1 = R([j*6-4 j*6-3 j*6-2 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([2 3 4 6],i) = (Cii)\tempvar;
        elseif BC3 == [0;1;1;1;0;1] % case 31 y z xt yt
            Cii = Cglobal([i*6-4 i*6-3 i*6-2 i*6-1],[i*6-4 i*6-3 i*6-2 i*6-1]);
            for j = 1:nNode
                Cij = Cglobal([i*6-4 i*6-3 i*6-2 i*6-1],[j*6-4 j*6-3 j*6-2 j*6-1]);
                R1 = R([j*6-4 j*6-3 j*6-2 j*6-1]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([2 3 4 5],i) = (Cii)\tempvar;
        elseif BC3 == [0;1;1;1;1;1] % case 32 y z xt yt zt
            Cii = Cglobal([i*6-4 i*6-3 i*6-2 i*6-1 i*6],[i*6-4 i*6-3 i*6-2 i*6-1 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-4 i*6-3 i*6-2 i*6-1 i*6],[j*6-4 j*6-3 j*6-2 j*6-1 j*6]);
                R1 = R([j*6-4 j*6-3 j*6-2 j*6-1 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([2 3 4 5 6],i) = (Cii)\tempvar;
        elseif BC3 == [1;0;0;0;0;0] % case 33 only x
            Cii = Cglobal(i*6-5,i*6-5);
            for j = 1:nNode
                Cij = Cglobal(i*6-5,j*6-5);
                R1 = R(j*6-5);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem(1,i) = (Cii)\tempvar;
        elseif BC3 == [1;0;0;0;0;1] % case 34 x zt
            Cii = Cglobal([i*6-5 i*6],[i*6-5 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6],[j*6-5 j*6]);
                R1 = R([j*6-5 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 6],i) = (Cii)\tempvar;
        elseif BC3 == [1;0;0;0;1;0] % case 35 x yt
            Cii = Cglobal([i*6-5 i*6-1],[i*6-5 i*6-1]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-1],[j*6-5 j*6-1]);
                R1 = R([j*6-5 j*6-1]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 5],i) = (Cii)\tempvar;
        elseif BC3 == [1;0;0;0;1;1] % case 36 x yt zt
            Cii = Cglobal([i*6-5 i*6-1 i*6],[i*6-5 i*6-1 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-1 i*6],[j*6-5 j*6-1 j*6]);
                R1 = R([j*6-5 j*6-1 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 5 6],i) = (Cii)\tempvar;
        elseif BC3 == [1;0;0;1;0;0] % case 37 x xt
            Cii = Cglobal([i*6-5 i*6-2],[i*6-5 i*6-2]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-2],[j*6-5 j*6-2]);
                R1 = R([j*6-5 j*6-2]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 4],i) = (Cii)\tempvar;
        elseif BC3 == [1;0;0;1;0;1] % case 38 x xt zt
            Cii = Cglobal([i*6-5 i*6-2 i*6],[i*6-5 i*6-2 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-2 i*6],[j*6-5 j*6-2 j*6]);
                R1 = R([j*6-5 j*6-2 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 4 6],i) = (Cii)\tempvar;
        elseif BC3 == [1;0;0;1;0;1] % case 39 x xt yt
            Cii = Cglobal([i*6-5 i*6-2 i*6-1],[i*6-5 i*6-2 i*6-1]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-2 i*6-1],[j*6-5 j*6-2 j*6-1]);
                R1 = R([j*6-5 j*6-2 j*6-1]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 4 5],i) = (Cii)\tempvar;
        elseif BC3 == [1;0;0;1;1;1] % case 40 x xt yt zt
            Cii = Cglobal([i*6-5 i*6-2 i*6-1 i*6],[i*6-5 i*6-2 i*6-1 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-2 i*6-1 i*6],[j*6-5 j*6-2 j*6-1 j*6]);
                R1 = R([j*6-5 j*6-2 j*6-1 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 4 5 6],i) = (Cii)\tempvar;
        elseif BC3 == [1;0;1;0;0;0] % case 41 x z
            Cii = Cglobal([i*6-5 i*6-3],[i*6-5 i*6-3]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-3],[j*6-5 j*6-3]);
                R1 = R([j*6-5 j*6-3]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 3],i) = (Cii)\tempvar;
        elseif BC3 == [1;0;1;0;0;1] % case 42 x z zt
            Cii = Cglobal([i*6-5 i*6-3 i*6],[i*6-5 i*6-3 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-3 i*6],[j*6-5 j*6-3 j*6]);
                R1 = R([j*6-5 j*6-3 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 3 6],i) = (Cii)\tempvar;
        elseif BC3 == [1;0;1;0;1;0] % case 43 x z yt
            Cii = Cglobal([i*6-5 i*6-3 i*6-1],[i*6-5 i*6-3 i*6-1]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-3 i*6-1],[j*6-5 j*6-3 j*6-1]);
                R1 = R([j*6-5 j*6-3 j*6-1]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 3 5],i) = (Cii)\tempvar;
        elseif BC3 == [1;0;0;0;1;1] % case 44 x z yt zt
            Cii = Cglobal([i*6-5 i*6-3 i*6-1 i*6],[i*6-5 i*6-3 i*6-1 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-3 i*6-1 i*6],[j*6-5 j*6-3 j*6-1 j*6]);
                R1 = R([j*6-5 j*6-3 j*6-1 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 3 5 6],i) = (Cii)\tempvar;
        elseif BC3 == [1;0;1;1;0;0] % case 45 x z xt
            Cii = Cglobal([i*6-5 i*6-3 i*6-2],[i*6-5 i*6-3 i*6-2]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-3 i*6-2],[j*6-5 j*6-3 j*6-2]);
                R1 = R([j*6-5 j*6-3 j*6-2]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 3 4],i) = (Cii)\tempvar;
        elseif BC3 == [1;0;0;1;0;1] % case 46 x z xt zt
            Cii = Cglobal([i*6-5 i*6-3 i*6-2 i*6],[i*6-5 i*6-3 i*6-2 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-3 i*6-2 i*6],[j*6-5 j*6-3 j*6-2 j*6]);
                R1 = R([j*6-5 j*6-3 j*6-2 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 3 4 6],i) = (Cii)\tempvar;
        elseif BC3 == [1;0;1;1;0;1] % case 47 x z xt yt
            Cii = Cglobal([i*6-5 i*6-3 i*6-2 i*6-1],[i*6-5 i*6-3 i*6-2 i*6-1]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-3 i*6-2 i*6-1],[j*6-5 j*6-3 j*6-2 j*6-1]);
                R1 = R([j*6-5 j*6-3 j*6-2 j*6-1]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 3 4 5],i) = (Cii)\tempvar;
        elseif BC3 == [1;0;1;1;1;1] % case 48 x z xt yt zt
            Cii = Cglobal([i*6-5 i*6-3 i*6-2 i*6-1 i*6],[i*6-5 i*6-3 i*6-2 i*6-1 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-3 i*6-2 i*6-1 i*6],[j*6-5 j*6-3 j*6-2 j*6-1 j*6]);
                R1 = R([j*6-5 j*6-3 j*6-2 j*6-1 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 3 4 5 6],i) = (Cii)\tempvar;
        elseif BC3 == [1;1;0;0;0;0] % case 49 x y
            Cii = Cglobal([i*6-5 i*6-4],[i*6-5 i*6-4]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-4],[j*6-5 j*6-4]);
                R1 = R([j*6-5 j*6-4]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 2],i) = (Cii)\tempvar;
        elseif BC3 == [1;1;0;0;0;1] % case 50 x y zt
            Cii = Cglobal([i*6-5 i*6-4 i*6],[i*6-5 i*6-4 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-4 i*6],[j*6-5 j*6-4 j*6]);
                R1 = R([j*6-5 j*6-4 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 2 6],i) = (Cii)\tempvar;
        elseif BC3 == [1;1;0;0;1;0] % case 51 x y yt
            Cii = Cglobal([i*6-5 i*6-4 i*6-1],[i*6-5 i*6-4 i*6-1]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-4 i*6-1],[j*6-5 j*6-4 j*6-1]);
                R1 = R([j*6-5 j*6-4 j*6-1]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 2 5],i) = (Cii)\tempvar;
        elseif BC3 == [1;1;0;0;1;1] % case 52 x y yt zt
            Cii = Cglobal([i*6-5 i*6-4 i*6-1 i*6],[i*6-5 i*6-4 i*6-1 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-4 i*6-1 i*6],[j*6-5 j*6-4 j*6-1 j*6]);
                R1 = R([j*6-5 j*6-4 j*6-1 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 2 5 6],i) = (Cii)\tempvar;
        elseif BC3 == [1;1;0;1;0;0] % case 53 x y xt
            Cii = Cglobal([i*6-5 i*6-4 i*6-2],[i*6-5 i*6-4 i*6-2]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-4 i*6-2],[j*6-5 j*6-4 j*6-2]);
                R1 = R([j*6-5 j*6-4 j*6-2]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 2 4],i) = (Cii)\tempvar;
        elseif BC3 == [1;1;0;1;0;1] % case 54 x y xt zt
            Cii = Cglobal([i*6-5 i*6-4 i*6-2 i*6],[i*6-5 i*6-4 i*6-2 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-4 i*6-2 i*6],[j*6-5 j*6-4 j*6-2 j*6]);
                R1 = R([j*6-5 j*6-4 j*6-2 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 2 4 6],i) = (Cii)\tempvar;
        elseif BC3 == [1;1;0;1;0;1] % case 55 x y xt yt
            Cii = Cglobal([i*6-5 i*6-4 i*6-2 i*6-1],[i*6-5 i*6-4 i*6-2 i*6-1]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-4 i*6-2 i*6-1],[j*6-5 j*6-4 j*6-2 j*6-1]);
                R1 = R([j*6-5 j*6-4 j*6-2 j*6-1]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 2 4 5],i) = (Cii)\tempvar;
        elseif BC3 == [1;1;0;1;1;1] % case 56 x y xt yt zt
            Cii = Cglobal([i*6-5 i*6-4 i*6-2 i*6-1 i*6],[i*6-5 i*6-4 i*6-2 i*6-1 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-4 i*6-2 i*6-1 i*6],[j*6-5 j*6-4 j*6-2 j*6-1 j*6]);
                R1 = R([j*6-5 j*6-4 j*6-2 j*6-1 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 2 4 5 6],i) = (Cii)\tempvar;
        elseif BC3 == [1;1;1;0;0;0] % case 57 x y z
            Cii = Cglobal([i*6-5 i*6-4 i*6-3],[i*6-5 i*6-4 i*6-3]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-4 i*6-3],[j*6-5 j*6-4 j*6-3]);
                R1 = R([j*6-5 j*6-4 j*6-3]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 2 3],i) = (Cii)\tempvar;
        elseif BC3 == [1;1;1;0;0;1] % case 58 x y z zt
            Cii = Cglobal([i*6-5 i*6-4 i*6-3 i*6],[i*6-5 i*6-4 i*6-3 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-4 i*6-3 i*6],[j*6-5 j*6-4 j*6-3 j*6]);
                R1 = R([j*6-5 j*6-4 j*6-3 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 2 3 6],i) = (Cii)\tempvar;
        elseif BC3 == [1;1;1;0;1;0] % case 59 x y z yt
            Cii = Cglobal([i*6-5 i*6-4 i*6-3 i*6-1],[i*6-5 i*6-4 i*6-3 i*6-1]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-4 i*6-3 i*6-1],[j*6-5 j*6-4 j*6-3 j*6-1]);
                R1 = R([j*6-5 j*6-4 j*6-3 j*6-1]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 2 3 5],i) = (Cii)\tempvar;
        elseif BC3 == [1;1;0;0;1;1] % case 60 x y z yt zt
            Cii = Cglobal([i*6-5 i*6-4 i*6-3 i*6-1 i*6],[i*6-5 i*6-4 i*6-3 i*6-1 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-4 i*6-3 i*6-1 i*6],[j*6-5 j*6-4 j*6-3 j*6-1 j*6]);
                R1 = R([j*6-5 j*6-4 j*6-3 j*6-1 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 2 3 5 6],i) = (Cii)\tempvar;
        elseif BC3 == [1;1;1;1;0;0] % case 61 x y z xt
            Cii = Cglobal([i*6-5 i*6-4 i*6-3 i*6-2],[i*6-5 i*6-4 i*6-3 i*6-2]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-4 i*6-3 i*6-2],[j*6-5 j*6-4 j*6-3 j*6-2]);
                R1 = R([j*6-5 j*6-4 j*6-3 j*6-2]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 2 3 4],i) = (Cii)\tempvar;
        elseif BC3 == [1;0;0;1;0;1] % case 62 x y z xt zt
            Cii = Cglobal([i*6-5 i*6-4 i*6-3 i*6-2 i*6],[i*6-5 i*6-4 i*6-3 i*6-2 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-4 i*6-3 i*6-2 i*6],[j*6-5 j*6-4 j*6-3 j*6-2 j*6]);
                R1 = R([j*6-5 j*6-4 j*6-3 j*6-2 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 2 3 4 6],i) = (Cii)\tempvar;
        elseif BC3 == [1;1;1;1;0;1] % case 63 x y z xt yt
            Cii = Cglobal([i*6-5 i*6-4 i*6-3 i*6-2 i*6-1],[i*6-5 i*6-4 i*6-3 i*6-2 i*6-1]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-4 i*6-3 i*6-2 i*6-1],[j*6-5 j*6-4 j*6-3 j*6-2 j*6-1]);
                R1 = R([j*6-5 j*6-4 j*6-3 j*6-2 j*6-1]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 2 3 4 5],i) = (Cii)\tempvar;
        elseif BC3 == [1;1;1;1;1;1] % case 64 x y z xt yt zt
            Cii = Cglobal([i*6-5 i*6-4 i*6-3 i*6-2 i*6-1 i*6],[i*6-5 i*6-4 i*6-3 i*6-2 i*6-1 i*6]);
            for j = 1:nNode
                Cij = Cglobal([i*6-5 i*6-4 i*6-3 i*6-2 i*6-1 i*6],[j*6-5 j*6-4 j*6-3 j*6-2 j*6-1 j*6]);
                R1 = R([j*6-5 j*6-4 j*6-3 j*6-2 j*6-1 j*6]);
                tempvar = (Cij*R1)+ tempvar;
            end
            Felem([1 2 3 4 5 6],i) = (Cii)\tempvar;
        end
        
    end


end


% Felem contains NaN's 
% this program chnages all the NaN's to 0 so that we can plot it using
% quiver and save in Felem1
Ft = Felem;
Felem1 = Ft;
for i = 1:size(Ft,1)
    for j = 1:size(Ft,2)
        if isnan(Felem1(i,j))
            Felem1(i,j) = 0;     
        end
    end
end

end

