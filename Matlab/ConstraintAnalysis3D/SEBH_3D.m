%{
Strain Enrgy based Homogenization code for 3d beam based geometries
Author : SreeKalyan Patiballa
Date : 3/17/2017
%}
clear
close all
clc 
clf

%%
%%%%% Load the geometry files in .mat format %%%%%%

x_cors = [0 0.5 0.5 0   0 0   0.5 0.5 0   0   0.5 0.5 0.5 0.5 0   0];
y_cors = [0 0   0.5 0.5 0 0   0   0.5 0.5 0   0   0   0.5 0.5 0.5 0.5];
z_cors = [0 0   0   0   0 0.5 0.5 0.5 0.5 0.5 0.5 0   0   0.5 0.5 0];

plot3(x_cors,y_cors,z_cors,'b');
hold on
X = [];
handles = []; 
% handles = Design1_a(X,handles);
% handles = Design2_a(X,handles);
% handles = Design4_bv2(X,handles);
% % handles = Design4_b_test(X,handles);
% handles = Design_ex1(X,handles);
% node = handles.node;
% elem = handles.elem; 
% T = handles.T;
% D = handles.D;
% Fr = handles.Fr;
% B = handles.B;
% R = handles.R;
% L = handles.L; 

%%%%%%%%%%%% Unity Integration %%%%%%%%%%%
tcpip = StartServer();
[vertString, edgeString, forceString] = ReadFromUnity(tcpip);
fprintf('%s\n', vertString);
fprintf('%s\n', edgeString);
fprintf('%s\n', forceString);

inputVertString = vertString;
inputEdgeString = edgeString;
inputForceString = forceString;

nmesh = 10;
[node, ~, ~, ~, ~] = getNodeCoordArray(vertString);
node(:,2:4) = 0.5 * node(:,2:4);
nnode = size(node,1);
elem = getElementArray(edgeString, nmesh);
force = getForceArray(forceString);
disp_scaling = 0.001;

handles = [];
handles.node = node;
handles.elem = elem;

handles = bin_categorize(handles);
T = handles.T;
D = handles.D;
Fr = handles.Fr;
B = handles.B;
R = handles.R;
L = handles.L; 
node = handles.node;
elem = handles.elem; 

%%%%%%%%%%%%%%%%%%%%%%%%%%%%
%%
%%%%%%%%%%%% Meshing %%%%%%%%%%%%%%%%%
NNODE = length(node(:,1)); % Number of nodes
nx = node(:,2); % X coordinates
ny = node(:,3); % Y coordinates
nz = node(:,4); % Z coordinates
NELEM = length(elem(:,1)); %Number of elements
ncon = elem(:,[2 3 1]); % connectivity
connectivity=ncon(:,1:2);
Be = elem(:,4); % linewidth
He = elem(:,5); %height
Y = elem(:,6);		% Young's Modulus
nmesh = elem(:,7); % number of divisions per element
nlines = NELEM;
npoints = NNODE;

% this loop meshes the elements depending on the nmesh 
for i=1:nlines,
    t(i,1) = ( nx(ncon(i,2))-nx(ncon(i,1)) );
    t(i,2) = ( ny(ncon(i,2))-ny(ncon(i,1)) );
    t(i,3) = ( nz(ncon(i,2))-nz(ncon(i,1)) );
    l(i) = sqrt(sum(t(i,:).^2));
    t(i,:) = t(i,:)./l(i);
    lnode=elem(i,3);
    for j=1:nmesh(i)-1,
        NNODE = NNODE+1;
        lp = l(i)/nmesh(i);
        nx(NNODE) = nx(ncon(i,1)) + j*lp*t(i,1);
        ny(NNODE) = ny(ncon(i,1)) + j*lp*t(i,2);
        nz(NNODE) = nz(ncon(i,1)) + j*lp*t(i,3);
        NELEM = NELEM+1;
        elem(NELEM,:) = elem(i,:);
        %elem(NELEM,1)=NELEM;
        if j==1 && j~=nmesh(i)-1,
            elem(i,3)=NNODE;
            elem(NELEM,2)=NNODE;
            elem(NELEM,3)=NNODE+1;
        end
        if j==nmesh(i)-1 && j~=1,
            elem(NELEM,2)=NNODE;
            elem(NELEM,3)=lnode;
            elem(NELEM,end)=0;
        end
        if j~=nmesh(i)-1 && j~=1,
            elem(NELEM,2)=NNODE;
            elem(NELEM,3)=NNODE+1;
            elem(NELEM,end)=0;
        end
        if j==1 && j==nmesh(i)-1,
            elem(NELEM,2)=NNODE;
            elem(NELEM,3)=lnode;
            elem(NELEM,end)=0;
            elem(i,3)=NNODE;
        end
    end
end

%Preprocessor
ncon = elem(:,[2 3 1]); % connectivity
Be = elem(:,4); % linewidth
He = elem(:,5); %height
Y = elem(:,6);		% Young's Modulus

% Force vector
F = zeros(6*NNODE,1); % we have no forces applied at any node 

% Compute the length of the elements
for ie=1:NELEM,
    ey = ncon(ie,1);
    jay = ncon(ie,2);
    Le(ie) = sqrt ( (nx(jay) - nx(ey))^2 + (ny(jay) - ny(ey))^2 + (nz(jay) - nz(ey))^2 );
end
t = zeros(NELEM,3);
l = zeros(NELEM,1);
for i = 1:size(ncon,1),
    t(i,1) = ( nx(ncon(i,2))-nx(ncon(i,1)) );
    t(i,2) = ( ny(ncon(i,2))-ny(ncon(i,1)) );
    t(i,3) = ( nz(ncon(i,2))-nz(ncon(i,1)) );
    l(i)=sqrt(sum(t(i,:).^2));
    t(i,:) = t(i,:)./sqrt(sum(t(i,:).^2));
end

% % Plot undeformed % %
figure(11)
for i = 1:NELEM,
   id1 = ncon(i,1);
   id2 = ncon(i,2);
   plot3([nx(id1) nx(id2)], [ny(id1) ny(id2)], [nz(id1) nz(id2)], 'b','Linewidth',4);hold on
end
hold on

for i=1:NNODE,
    text(nx(i),ny(i),nz(i),num2str(i),'Color','red','FontSize',14);hold on;
end
xlabel('x');
ylabel('y');
zlabel('z');
axis equal
grid

% FEM [Have to run FEM for all the 9 cases for 3D models]
% inp = dispnode1;
utotal=zeros(NNODE*3,6); 
%%% remember : it is important that you have node defined in each element(
%%% more divisions than 1) 

% Nine boundary conditions 
%{
In solid mechanics, the general rule for a symmetry displacement condition is that the displacement
vector component perpendicular to the plane is zero and the rotational vector components parallel
to the plane are zero
%}


%%%% this functions calculates all the dof's of each face %%%%
[ temp_T, temp_D, temp_F, temp_B, temp_R, temp_L ] = dof_calc( T,D,Fr,B,R,L );
[ BC1, BC2, BC3, BC4, BC5, BC6, BC7, BC8, BC9 ] = bc_calc( T,D,Fr,B,R,L,NNODE );


%case 1
% dispID_1 = [temp_R(:,1)' temp_L(:,1)' temp_F(:,2)' temp_B(:,2)' temp_T(:,3)' temp_D(:,3)' ...
%     temp_R(:,5)' temp_R(:,6)' temp_L(:,5)' temp_L(:,6)' temp_F(:,4)' temp_F(:,6)' temp_B(:,4)' temp_B(:,6)' temp_T(:,4)' temp_T(:,5)' temp_D(:,4)' temp_D(:,5)']; 
% dispVal_1 = [ones(1,size(temp_R,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_F,1)) zeros(1,size(temp_B,1)) zeros(1,size(temp_T,1)) zeros(1,size(temp_D,1)) ...
%     zeros(1,size(temp_R,1)) zeros(1,size(temp_R,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_F,1)) zeros(1,size(temp_F,1)) zeros(1,size(temp_B,1)) zeros(1,size(temp_B,1)) zeros(1,size(temp_T,1)) zeros(1,size(temp_T,1)) zeros(1,size(temp_D,1)) zeros(1,size(temp_D,1))];
dispID_1 = [temp_R(:,1)' temp_L(:,1)' temp_F(:,2)' temp_D(:,3)' ...
    temp_R(:,5)' temp_R(:,6)' temp_L(:,5)' temp_L(:,6)' temp_F(:,4)' temp_F(:,6)' temp_B(:,4)' temp_B(:,6)' temp_T(:,4)' temp_T(:,5)' temp_D(:,4)' temp_D(:,5)']; 
dispVal_1 = [ones(1,size(temp_R,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_F,1)) zeros(1,size(temp_D,1)) ...
    zeros(1,size(temp_R,1)) zeros(1,size(temp_R,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_F,1)) zeros(1,size(temp_F,1)) zeros(1,size(temp_B,1)) zeros(1,size(temp_B,1)) zeros(1,size(temp_T,1)) zeros(1,size(temp_T,1)) zeros(1,size(temp_D,1)) zeros(1,size(temp_D,1))];

%case 2
dispID_2 = [temp_R(:,1)' temp_L(:,1)' temp_F(:,2)' temp_B(:,2)' temp_T(:,3)' temp_D(:,3)' ...
    temp_R(:,5)' temp_R(:,6)' temp_L(:,5)' temp_L(:,6)' temp_F(:,4)' temp_F(:,6)' temp_B(:,4)' temp_B(:,6)' temp_T(:,4)' temp_T(:,5)' temp_D(:,4)' temp_D(:,5)']; 
dispVal_2 = [zeros(1,size(temp_R,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_F,1)) ones(1,size(temp_B,1)) zeros(1,size(temp_T,1)) zeros(1,size(temp_D,1)) ...
    zeros(1,size(temp_R,1)) zeros(1,size(temp_R,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_F,1)) zeros(1,size(temp_F,1)) zeros(1,size(temp_B,1)) zeros(1,size(temp_B,1)) zeros(1,size(temp_T,1)) zeros(1,size(temp_T,1)) zeros(1,size(temp_D,1)) zeros(1,size(temp_D,1))];

%case 3
dispID_3 = [temp_R(:,1)' temp_L(:,1)' temp_F(:,2)' temp_B(:,2)' temp_T(:,3)' temp_D(:,3)' ...
    temp_R(:,5)' temp_R(:,6)' temp_L(:,5)' temp_L(:,6)' temp_F(:,4)' temp_F(:,6)' temp_B(:,4)' temp_B(:,6)' temp_T(:,4)' temp_T(:,5)' temp_D(:,4)' temp_D(:,5)']; 
dispVal_3 = [zeros(1,size(temp_R,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_F,1)) zeros(1,size(temp_B,1)) ones(1,size(temp_T,1)) zeros(1,size(temp_D,1)) ...
    zeros(1,size(temp_R,1)) zeros(1,size(temp_R,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_F,1)) zeros(1,size(temp_F,1)) zeros(1,size(temp_B,1)) zeros(1,size(temp_B,1)) zeros(1,size(temp_T,1)) zeros(1,size(temp_T,1)) zeros(1,size(temp_D,1)) zeros(1,size(temp_D,1))];

%case 4
dispID_4 = [temp_R(:,1)' temp_L(:,1)' temp_F(:,3)' temp_B(:,3)' temp_T(:,2)' temp_D(:,2)' ...
    temp_R(:,5)' temp_R(:,6)' temp_L(:,5)' temp_L(:,6)' temp_F(:,4)' temp_F(:,6)' temp_B(:,4)' temp_B(:,6)' temp_T(:,4)' temp_T(:,5)' temp_D(:,4)' temp_D(:,5)']; 
dispVal_4 = [zeros(1,size(temp_R,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_F,1)) 0.5*ones(1,size(temp_B,1)) 0.5*ones(1,size(temp_T,1)) zeros(1,size(temp_D,1)) ...
    zeros(1,size(temp_R,1)) zeros(1,size(temp_R,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_F,1)) zeros(1,size(temp_F,1)) zeros(1,size(temp_B,1)) zeros(1,size(temp_B,1)) zeros(1,size(temp_T,1)) zeros(1,size(temp_T,1)) zeros(1,size(temp_D,1)) zeros(1,size(temp_D,1))];

%case 5
dispID_5 = [temp_R(:,3)' temp_L(:,3)' temp_F(:,2)' temp_B(:,2)' temp_T(:,1)' temp_D(:,1)' ...
    temp_R(:,5)' temp_R(:,6)' temp_L(:,5)' temp_L(:,6)' temp_F(:,4)' temp_F(:,6)' temp_B(:,4)' temp_B(:,6)' temp_T(:,4)' temp_T(:,5)' temp_D(:,4)' temp_D(:,5)']; 
dispVal_5 = [0.5*ones(1,size(temp_R,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_F,1)) zeros(1,size(temp_B,1)) 0.5*ones(1,size(temp_T,1)) zeros(1,size(temp_D,1)) ...
    zeros(1,size(temp_R,1)) zeros(1,size(temp_R,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_F,1)) zeros(1,size(temp_F,1)) zeros(1,size(temp_B,1)) zeros(1,size(temp_B,1)) zeros(1,size(temp_T,1)) zeros(1,size(temp_T,1)) zeros(1,size(temp_D,1)) zeros(1,size(temp_D,1))];

%case 6
dispID_6 = [temp_R(:,2)' temp_L(:,2)' temp_F(:,1)' temp_B(:,1)' temp_T(:,3)' temp_D(:,3)' ...
    temp_R(:,5)' temp_R(:,6)' temp_L(:,5)' temp_L(:,6)' temp_F(:,4)' temp_F(:,6)' temp_B(:,4)' temp_B(:,6)' temp_T(:,4)' temp_T(:,5)' temp_D(:,4)' temp_D(:,5)']; 
dispVal_6 = [0.5*ones(1,size(temp_R,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_F,1)) 0.5*ones(1,size(temp_B,1)) zeros(1,size(temp_T,1)) zeros(1,size(temp_D,1)) ...
    zeros(1,size(temp_R,1)) zeros(1,size(temp_R,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_F,1)) zeros(1,size(temp_F,1)) zeros(1,size(temp_B,1)) zeros(1,size(temp_B,1)) zeros(1,size(temp_T,1)) zeros(1,size(temp_T,1)) zeros(1,size(temp_D,1)) zeros(1,size(temp_D,1))];

%case 7
dispID_7 = [temp_R(:,1)' temp_L(:,1)' temp_F(:,2)' temp_B(:,2)' temp_T(:,3)' temp_D(:,3)' ...
    temp_R(:,5)' temp_R(:,6)' temp_L(:,5)' temp_L(:,6)' temp_F(:,4)' temp_F(:,6)' temp_B(:,4)' temp_B(:,6)' temp_T(:,4)' temp_T(:,5)' temp_D(:,4)' temp_D(:,5)']; 
dispVal_7 = [ones(1,size(temp_R,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_F,1)) ones(1,size(temp_B,1)) zeros(1,size(temp_T,1)) zeros(1,size(temp_D,1)) ...
    zeros(1,size(temp_R,1)) zeros(1,size(temp_R,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_F,1)) zeros(1,size(temp_F,1)) zeros(1,size(temp_B,1)) zeros(1,size(temp_B,1)) zeros(1,size(temp_T,1)) zeros(1,size(temp_T,1)) zeros(1,size(temp_D,1)) zeros(1,size(temp_D,1))];

%case 8
dispID_8 = [temp_R(:,1)' temp_L(:,1)' temp_F(:,2)' temp_B(:,2)' temp_T(:,3)' temp_D(:,3)' ...
    temp_R(:,5)' temp_R(:,6)' temp_L(:,5)' temp_L(:,6)' temp_F(:,4)' temp_F(:,6)' temp_B(:,4)' temp_B(:,6)' temp_T(:,4)' temp_T(:,5)' temp_D(:,4)' temp_D(:,5)']; 
dispVal_8 = [ones(1,size(temp_R,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_F,1)) zeros(1,size(temp_B,1)) ones(1,size(temp_T,1)) zeros(1,size(temp_D,1)) ...
    zeros(1,size(temp_R,1)) zeros(1,size(temp_R,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_F,1)) zeros(1,size(temp_F,1)) zeros(1,size(temp_B,1)) zeros(1,size(temp_B,1)) zeros(1,size(temp_T,1)) zeros(1,size(temp_T,1)) zeros(1,size(temp_D,1)) zeros(1,size(temp_D,1))];

%case 9
dispID_9 = [temp_R(:,1)' temp_L(:,1)' temp_F(:,2)' temp_B(:,2)' temp_T(:,3)' temp_D(:,3)' ...
    temp_R(:,5)' temp_R(:,6)' temp_L(:,5)' temp_L(:,6)' temp_F(:,4)' temp_F(:,6)' temp_B(:,4)' temp_B(:,6)' temp_T(:,4)' temp_T(:,5)' temp_D(:,4)' temp_D(:,5)']; 
dispVal_9 = [zeros(1,size(temp_R,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_F,1)) ones(1,size(temp_B,1)) ones(1,size(temp_T,1)) zeros(1,size(temp_D,1)) ...
    zeros(1,size(temp_R,1)) zeros(1,size(temp_R,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_L,1)) zeros(1,size(temp_F,1)) zeros(1,size(temp_F,1)) zeros(1,size(temp_B,1)) zeros(1,size(temp_B,1)) zeros(1,size(temp_T,1)) zeros(1,size(temp_T,1)) zeros(1,size(temp_D,1)) zeros(1,size(temp_D,1))];



dispsort1 = sortrows([dispID_1' dispVal_1'],1);% if you don't sort you will have problems while doing F-KU
dispsort_unique1 = unique(dispsort1,'rows');
dispID1 = dispsort_unique1(:,1)';
dispVal1 = dispsort_unique1(:,2)';

dispsort2 = sortrows([dispID_2' dispVal_2'],1);% if you don't sort you will have problems while doing F-KU
dispsort_unique2 = unique(dispsort2,'rows');
dispID2 = dispsort_unique2(:,1)';
dispVal2 = dispsort_unique2(:,2)';

dispsort3 = sortrows([dispID_3' dispVal_3'],1);% if you don't sort you will have problems while doing F-KU
dispsort_unique3 = unique(dispsort3,'rows');
dispID3 = dispsort_unique3(:,1)';
dispVal3 = dispsort_unique3(:,2)';

dispsort4 = sortrows([dispID_4' dispVal_4'],1);% if you don't sort you will have problems while doing F-KU
dispsort_unique4 = unique(dispsort4,'rows');
dispID4 = dispsort_unique4(:,1)';
dispVal4 = dispsort_unique4(:,2)';

dispsort5 = sortrows([dispID_5' dispVal_5'],1);% if you don't sort you will have problems while doing F-KU
dispsort_unique5 = unique(dispsort5,'rows');
dispID5 = dispsort_unique5(:,1)';
dispVal5 = dispsort_unique5(:,2)';

dispsort6 = sortrows([dispID_6' dispVal_6'],1);% if you don't sort you will have problems while doing F-KU
dispsort_unique6 = unique(dispsort6,'rows');
dispID6 = dispsort_unique6(:,1)';
dispVal6 = dispsort_unique6(:,2)';

dispsort7 = sortrows([dispID_7' dispVal_7'],1);% if you don't sort you will have problems while doing F-KU
dispsort_unique7 = unique(dispsort7,'rows');
dispID7 = dispsort_unique7(:,1)';
dispVal7 = dispsort_unique7(:,2)';

dispsort8 = sortrows([dispID_8' dispVal_8'],1);% if you don't sort you will have problems while doing F-KU
dispsort_unique8 = unique(dispsort8,'rows');
dispID8 = dispsort_unique8(:,1)';
dispVal8 = dispsort_unique8(:,2)';

dispsort9 = sortrows([dispID_9' dispVal_9'],1);% if you don't sort you will have problems while doing F-KU
dispsort_unique9 = unique(dispsort9,'rows');
dispID9 = dispsort_unique9(:,1)';
dispVal9 = dispsort_unique9(:,2)';



tic
[u1,R1,K1,SE1] = fem_3d(Be,He,Le,Y,t,ncon,NELEM,NNODE,F,dispID1,dispVal1);
[u2,R2,K2,SE2] = fem_3d(Be,He,Le,Y,t,ncon,NELEM,NNODE,F,dispID2,dispVal2);
[u3,R3,K3,SE3] = fem_3d(Be,He,Le,Y,t,ncon,NELEM,NNODE,F,dispID3,dispVal3);
[u4,R4,K4,SE4] = fem_3d(Be,He,Le,Y,t,ncon,NELEM,NNODE,F,dispID4,dispVal4);
[u5,R5,K5,SE5] = fem_3d(Be,He,Le,Y,t,ncon,NELEM,NNODE,F,dispID5,dispVal5);
[u6,R6,K6,SE6] = fem_3d(Be,He,Le,Y,t,ncon,NELEM,NNODE,F,dispID6,dispVal6);
[u7,R7,K7,SE7] = fem_3d(Be,He,Le,Y,t,ncon,NELEM,NNODE,F,dispID7,dispVal7);
[u8,R8,K8,SE8] = fem_3d(Be,He,Le,Y,t,ncon,NELEM,NNODE,F,dispID8,dispVal8);
[u9,R9,K9,SE9] = fem_3d(Be,He,Le,Y,t,ncon,NELEM,NNODE,F,dispID9,dispVal9);
% [utotal,FTransfer,MTransfer,Rin,C] = fea_3D(Be,He,Le,Y,ncon,NELEM,NNODE,F,dispID,dispID,dispVal,t,inp,en);
toc
dim = 3; 
tic
[Cglobal1,Felem1]=LoadFlow_general(NNODE,NELEM,K1,BC1',R1,dim);
[Cglobal2,Felem2]=LoadFlow_general(NNODE,NELEM,K2,BC2',R2,dim); 
[Cglobal3,Felem3]=LoadFlow_general(NNODE,NELEM,K3,BC3',R3,dim); 
[Cglobal4,Felem4]=LoadFlow_general(NNODE,NELEM,K4,BC4',R4,dim); 
[Cglobal5,Felem5]=LoadFlow_general(NNODE,NELEM,K5,BC5',R5,dim); 
[Cglobal6,Felem6]=LoadFlow_general(NNODE,NELEM,K6,BC6',R6,dim); 
[Cglobal7,Felem7]=LoadFlow_general(NNODE,NELEM,K7,BC7',R7,dim); 
[Cglobal8,Felem8]=LoadFlow_general(NNODE,NELEM,K8,BC8',R8,dim); 
[Cglobal9,Felem9]=LoadFlow_general(NNODE,NELEM,K9,BC9',R9,dim); 
toc

% Transferred forces and moments
FT1 = Felem1(1:3,:); MT1 = Felem1(4:6,:); 
FT2 = Felem2(1:3,:); MT2 = Felem2(4:6,:); 
FT3 = Felem3(1:3,:); MT3 = Felem3(4:6,:); 
FT4 = Felem4(1:3,:); MT4 = Felem4(4:6,:); 
FT5 = Felem5(1:3,:); MT5 = Felem5(4:6,:); 
FT6 = Felem6(1:3,:); MT6 = Felem6(4:6,:); 
FT7 = Felem7(1:3,:); MT7 = Felem7(4:6,:); 
FT8 = Felem8(1:3,:); MT8 = Felem8(4:6,:); 
FT9 = Felem9(1:3,:); MT9 = Felem9(4:6,:); 


for i = 1:NNODE
    qu1(i) = FT1(1,i)/norm(FT1(:,i)); 
    qv1(i) = FT1(2,i)/norm(FT1(:,i)); 
    qw1(i) = FT1(3,i)/norm(FT1(:,i));    
end
for i = 1:NNODE
    qu2(i) = FT2(1,i)/norm(FT2(:,i)); 
    qv2(i) = FT2(2,i)/norm(FT2(:,i)); 
    qw2(i) = FT2(3,i)/norm(FT2(:,i));    
end
for i = 1:NNODE
    qu3(i) = FT3(1,i)/norm(FT3(:,i)); 
    qv3(i) = FT3(2,i)/norm(FT3(:,i)); 
    qw3(i) = FT3(3,i)/norm(FT3(:,i));    
end
for i = 1:NNODE
    qu4(i) = FT4(1,i)/norm(FT4(:,i)); 
    qv4(i) = FT4(2,i)/norm(FT4(:,i)); 
    qw4(i) = FT4(3,i)/norm(FT4(:,i));    
end
for i = 1:NNODE
    qu5(i) = FT5(1,i)/norm(FT5(:,i)); 
    qv5(i) = FT5(2,i)/norm(FT5(:,i)); 
    qw5(i) = FT5(3,i)/norm(FT5(:,i));    
end
for i = 1:NNODE
    qu6(i) = FT6(1,i)/norm(FT6(:,i)); 
    qv6(i) = FT6(2,i)/norm(FT6(:,i)); 
    qw6(i) = FT6(3,i)/norm(FT6(:,i));    
end
for i = 1:NNODE
    qu7(i) = FT7(1,i)/norm(FT7(:,i)); 
    qv7(i) = FT7(2,i)/norm(FT7(:,i)); 
    qw7(i) = FT7(3,i)/norm(FT7(:,i));    
end
for i = 1:NNODE
    qu8(i) = FT8(1,i)/norm(FT8(:,i)); 
    qv8(i) = FT8(2,i)/norm(FT8(:,i)); 
    qw8(i) = FT8(3,i)/norm(FT8(:,i));    
end
for i = 1:NNODE
    qu9(i) = FT9(1,i)/norm(FT9(:,i)); 
    qv9(i) = FT9(2,i)/norm(FT9(:,i)); 
    qw9(i) = FT9(3,i)/norm(FT9(:,i));    
end
% for i=1:NELEM,
%     eye = ncon(i,1);jay=ncon(i,2);
%     if FT(1,i)~=-11
%         quiver3(nx(eye)/2+nx(jay)/2, ny(eye)/2+ny(jay)/2,nz(eye)/2+nz(jay)/2,FT(1,i)/norm(FT(:,i)),FT(2,i)/norm(FT(:,i)),FT(3,i)/norm(FT(:,i)),0.1,'k','Linewidth',2);hold on
%     end
% end

C1111 = 2 * (SE1) ;
C2222 = 2 * (SE2) ;
C3333 = 2 * (SE3) ; 
C2323 = 2 * (SE4) ; 
C1313 = 2 * (SE5) ;
C1212 = 2 * (SE6) ;
C1122 = ((SE7) - (SE1) - (SE2)) ;
C1133 = ((SE8) - (SE1) - (SE3)) ;
C2233 = ((SE9) - (SE2) - (SE3)) ;

handles.H = [ C1111 C1122 C1133 0 0 0 ;
    C1122 C2222 C2233 0 0 0;
    C1133 C2233 C3333 0 0 0;
    0 0 0 C2323 0 0 ;
    0 0 0 0 C1313 0 ;
    0 0 0 0 0 C1212];
handles.HNorm = (handles.H)./C1111; 

handles.nu12 = C1122/ C1111; 
handles.nu13 = C1133/ C1111;
handles.nu23 = C2233/ C2222; 

%%% this plots both deformed and undeformed configurations for all 9
%%% boundary cases %%%%%%%%%
for j = 1:9
    figure(j)
    u = [u1 u2 u3 u4 u5 u6 u7 u8 u9];
    qu = [qu1' qu2' qu3' qu4' qu5' qu6' qu7' qu8' qu9'];
    qv = [qv1' qv2' qv3' qv4' qv5' qv6' qv7' qv8' qv9']; 
    qw = [qw1' qw2' qw3' qw4' qw5' qw6' qw7' qw8' qw9']; 
    for i = 1:NELEM,
        id1 = ncon(i,1);
        id2 = ncon(i,2);
        uid1 = 6*(id1-1) + 1;
        uid2 = 6*(id2-1) + 1;
        disp_scaling = 0.05; 
        plot3([nx(id1) nx(id2)], [ny(id1) ny(id2)], [nz(id1) nz(id2)], 'b','Linewidth',4);hold on
        plot3([nx(id1)+disp_scaling*u(uid1,j) nx(id2)+disp_scaling*u(uid2,j)], [ny(id1)+disp_scaling*u(uid1+1,j) ny(id2)+disp_scaling*u(uid2+1,j)],[nz(id1)+disp_scaling*u(uid1+2,j) nz(id2)+disp_scaling*u(uid2+2,j)],'-r'); hold on        
    end
    for i=1:NNODE,
        text(nx(i),ny(i),nz(i),num2str(i),'Color','red','FontSize',14);hold on;
    end
    quiver3(nx,ny,nz,qu(:,j),qv(:,j),qw(:,j),'color','r','LineWidth',2,'MarkerSize',10,'MaxHeadSize',1.5,'AutoScale','on');hold on
    xlabel('x');
    ylabel('y');
    zlabel('z');
    axis equal
    grid
    hold off
end


