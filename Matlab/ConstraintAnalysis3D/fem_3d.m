function [ u,R,Ksing,SE,Lf] = fem_3d( Be,He,Le,Y,t,ncon,NELEM,NNODE,F,dispID,dispVal)
% input:
% Be = width of the element
% He = height of the element
% Le = length of the element
% Y = Young's modulus
% ncon = connectivity
% NELEM, NNODE = number of elements and number of nodes
% dispID = displacement element number
% F = force vector
% t = orientation direction 1
% inp = 
% em = 
%
% outputs:
% utotal = displacement vector
% FTransfer = load transfer 
% Mtransfer = moment transfer
% Rin = 
% Cin = 
% Rdisp = reaction forces at the nodes that are fixed
% R = reaction forces 
% Ksing = global stiffness matrix before application of boundary conditions
% SE = strain energy
%
% This function creates the global stiffness matrix from local
% mass and stiffness matrices defined in Przemieniecki p. 79 and p. 294)
% for 6 degrees of freedom per node.

%%%%%%%%%%%%%%%%%%%%%%%%
nu = 0.29;			% Poisson Ratio

K = zeros(6*NNODE,6*NNODE);	% Initialize global stiffness matrix
k = zeros(12,12);		% Initialize local stiffness matrix

%%%%

%Rx=[1 0 0;0 cos -sin 0;0 sin cos]
%Ry=[cos 0 sin;0 1 0;-sin 0 cos]
%Rx=[cos -sin 0;sin cos 0;0 0 1]

for ie=1:NELEM,
    eye = ncon(ie,1);
    jay = ncon(ie,2);
    % Form the transformation matrix, Lambda.
    L = Le(ie);
    b = Be(ie);
    h = He(ie);
    A = b*h;
    J = b*h*(b^2+h^2)/12;
    Iy= b*h^3/12;   % beam (x-z plane as in-plane) h*b^3/12;
    Iz= h*b^3/12;   % beam (x-y plane as in-plane) h*b^3/12;
    E = Y(ie);
    G = E/(2*(1+nu));		% Shear Modulus
    
    %     lox = t(ie,1);   loy = t(ie,2);   loz = t(ie,3);
    %     mox = n1(ie,1);  moy = n1(ie,2);  moz = n1(ie,3);
    %     nox = n2(ie,1);  noy = n2(ie,2);  noz = n2(ie,3);
    %%%%%%%%%%%
    %     lox = t(ie,1);  loy = n1(ie,1);  loz = n2(ie,1);
    %     mox = t(ie,2);  moy = n1(ie,2);  moz = n2(ie,2);
    %     nox = t(ie,3);  noy = n1(ie,3);  noz = n2(ie,3);
    %%%%%%%%%%%
    B=null(t(ie,:))';
    N1=B(1,:);N2=cross(t(ie,:),N1);
    lox = t(ie,1);  loy = N1(1);  loz = N2(1);
    mox = t(ie,2);  moy = N1(2);  moz = N2(2);
    nox = t(ie,3);  noy = N1(3);  noz = N2(3);
    %%%%%%%%%%%
    
    Lambda = [ lox mox nox   0   0   0   0   0   0  0   0   0  ; ...
        loy moy noy   0   0   0   0   0   0  0   0   0  ; ...
        loz moz noz   0   0   0   0   0   0  0   0   0  ; ...
        0   0   0   lox mox nox  0   0   0  0   0   0  ; ...
        0   0   0   loy moy noy  0   0   0  0   0   0  ; ...
        0   0   0   loz moz noz  0   0   0  0   0   0  ; ...
        0   0   0    0   0   0  lox mox nox 0   0   0  ; ...
        0   0   0    0   0   0  loy moy noy 0   0   0  ; ...
        0   0   0    0   0   0  loz moz noz 0   0   0  ; ...
        0   0   0    0   0   0   0   0   0 lox mox nox ; ...
        0   0   0    0   0   0   0   0   0 loy moy noy ; ...
        0   0   0    0   0   0   0   0   0 loz moz noz ];
    
    
    % Form local element matrix
    
    k(1,1)  =  E*A/L;
    k(2,2)  =  12*E*Iz/L^3;	k(2,6)   =  6*E*Iz/L^2;
    k(3,3)  =  12*E*Iy/L^3;	k(3,5)   = -6*E*Iy/L^2;
    k(4,4)  =  G*J/L;
    k(5,3)  = -6*E*Iy/L^2;	k(5,5)   =  4*E*Iy/L;
    k(6,2)  =  6*E*Iz/L^2;      	k(6,6)   =  4*E*Iz/L;
    k(7,1)  = -k(1,1);		k(1,7)   =  k(7,1);
    k(8,2)  = -k(2,2);		k(2,8)   =  k(8,2);
    k(8,6)  = -k(6,2);		k(6,8)   =  k(8,6);
    k(9,3)  = -k(3,3);		k(3,9)   =  k(9,3);
    k(9,5)  = -k(5,3);		k(5,9)   =  k(9,5);
    k(10,4) = -k(4,4);		k(4,10)	 =  k(10,4);
    k(11,3) =  k(5,3);		k(3,11)	 =  k(11,3);
    k(11,5) =  2*E*Iy/L;		k(5,11)  =  k(11,5);
    k(12,2) =  k(6,2);		k(2,12)  =  k(12,2);
    k(12,6) =  2*E*Iz/L;		k(6,12)  =  k(12,6);
    k(7,7)  =  k(1,1);
    k(8,8)  =  k(2,2);		k(8,12)  = -k(2,6);
    k(9,9)  =  k(3,3); 		k(9,11)  = -k(3,5);
    k(10,10)=  k(4,4);
    k(11,9) = -k(5,3);		k(11,11) =  k(5,5);
    k(12,8) = -k(6,2);		k(12,12) =  k(6,6);
    
    klocal = Lambda' * k * Lambda;
    KK(:,:,ie)=klocal;
    %     klocal
    %     pause
    % Form ID matrix to assemble klocal into the global stiffness matrix, K.
    id1  = 6*(eye-1) + 1;
    id2  = id1 + 1;
    id3  = id2 + 1;
    id4  = id3 + 1;
    id5  = id4 + 1;
    id6  = id5 + 1;
    id7  = 6*(jay-1) + 1;
    id8  = id7 + 1;
    id9  = id8 + 1;
    id10 = id9 + 1;
    id11 = id10 + 1;
    id12 = id11 + 1;
    
    
    K(id1,id1)  = K(id1,id1)  + klocal(1,1);
    K(id1,id2)  = K(id1,id2)  + klocal(1,2);
    K(id1,id3)  = K(id1,id3)  + klocal(1,3);
    K(id1,id4)  = K(id1,id4)  + klocal(1,4);
    K(id1,id5)  = K(id1,id5)  + klocal(1,5);
    K(id1,id6)  = K(id1,id6)  + klocal(1,6);
    K(id1,id7)  = K(id1,id7)  + klocal(1,7);
    K(id1,id8)  = K(id1,id8)  + klocal(1,8);
    K(id1,id9)  = K(id1,id9)  + klocal(1,9);
    K(id1,id10) = K(id1,id10) + klocal(1,10);
    K(id1,id11) = K(id1,id11) + klocal(1,11);
    K(id1,id12) = K(id1,id12) + klocal(1,12);
    
    K(id2,id1)  = K(id2,id1)  + klocal(2,1);
    K(id2,id2)  = K(id2,id2)  + klocal(2,2);
    K(id2,id3)  = K(id2,id3)  + klocal(2,3);
    K(id2,id4)  = K(id2,id4)  + klocal(2,4);
    K(id2,id5)  = K(id2,id5)  + klocal(2,5);
    K(id2,id6)  = K(id2,id6)  + klocal(2,6);
    K(id2,id7)  = K(id2,id7)  + klocal(2,7);
    K(id2,id8)  = K(id2,id8)  + klocal(2,8);
    K(id2,id9)  = K(id2,id9)  + klocal(2,9);
    K(id2,id10) = K(id2,id10) + klocal(2,10);
    K(id2,id11) = K(id2,id11) + klocal(2,11);
    K(id2,id12) = K(id2,id12) + klocal(2,12);
    
    K(id3,id1)  = K(id3,id1)  + klocal(3,1);
    K(id3,id2)  = K(id3,id2)  + klocal(3,2);
    K(id3,id3)  = K(id3,id3)  + klocal(3,3);
    K(id3,id4)  = K(id3,id4)  + klocal(3,4);
    K(id3,id5)  = K(id3,id5)  + klocal(3,5);
    K(id3,id6)  = K(id3,id6)  + klocal(3,6);
    K(id3,id7)  = K(id3,id7)  + klocal(3,7);
    K(id3,id8)  = K(id3,id8)  + klocal(3,8);
    K(id3,id9)  = K(id3,id9)  + klocal(3,9);
    K(id3,id10) = K(id3,id10) + klocal(3,10);
    K(id3,id11) = K(id3,id11) + klocal(3,11);
    K(id3,id12) = K(id3,id12) + klocal(3,12);
    
    K(id4,id1)  = K(id4,id1)  + klocal(4,1);
    K(id4,id2)  = K(id4,id2)  + klocal(4,2);
    K(id4,id3)  = K(id4,id3)  + klocal(4,3);
    K(id4,id4)  = K(id4,id4)  + klocal(4,4);
    K(id4,id5)  = K(id4,id5)  + klocal(4,5);
    K(id4,id6)  = K(id4,id6)  + klocal(4,6);
    K(id4,id7)  = K(id4,id7)  + klocal(4,7);
    K(id4,id8)  = K(id4,id8)  + klocal(4,8);
    K(id4,id9)  = K(id4,id9)  + klocal(4,9);
    K(id4,id10) = K(id4,id10) + klocal(4,10);
    K(id4,id11) = K(id4,id11) + klocal(4,11);
    K(id4,id12) = K(id4,id12) + klocal(4,12);
    
    K(id5,id1)  = K(id5,id1)  + klocal(5,1);
    K(id5,id2)  = K(id5,id2)  + klocal(5,2);
    K(id5,id3)  = K(id5,id3)  + klocal(5,3);
    K(id5,id4)  = K(id5,id4)  + klocal(5,4);
    K(id5,id5)  = K(id5,id5)  + klocal(5,5);
    K(id5,id6)  = K(id5,id6)  + klocal(5,6);
    K(id5,id7)  = K(id5,id7)  + klocal(5,7);
    K(id5,id8)  = K(id5,id8)  + klocal(5,8);
    K(id5,id9)  = K(id5,id9)  + klocal(5,9);
    K(id5,id10) = K(id5,id10) + klocal(5,10);
    K(id5,id11) = K(id5,id11) + klocal(5,11);
    K(id5,id12) = K(id5,id12) + klocal(5,12);
    
    K(id6,id1)  = K(id6,id1)  + klocal(6,1);
    K(id6,id2)  = K(id6,id2)  + klocal(6,2);
    K(id6,id3)  = K(id6,id3)  + klocal(6,3);
    K(id6,id4)  = K(id6,id4)  + klocal(6,4);
    K(id6,id5)  = K(id6,id5)  + klocal(6,5);
    K(id6,id6)  = K(id6,id6)  + klocal(6,6);
    K(id6,id7)  = K(id6,id7)  + klocal(6,7);
    K(id6,id8)  = K(id6,id8)  + klocal(6,8);
    K(id6,id9)  = K(id6,id9)  + klocal(6,9);
    K(id6,id10) = K(id6,id10) + klocal(6,10);
    K(id6,id11) = K(id6,id11) + klocal(6,11);
    K(id6,id12) = K(id6,id12) + klocal(6,12);
    
    
    K(id7,id1)  = K(id7,id1)  + klocal(7,1);
    K(id7,id2)  = K(id7,id2)  + klocal(7,2);
    K(id7,id3)  = K(id7,id3)  + klocal(7,3);
    K(id7,id4)  = K(id7,id4)  + klocal(7,4);
    K(id7,id5)  = K(id7,id5)  + klocal(7,5);
    K(id7,id6)  = K(id7,id6)  + klocal(7,6);
    K(id7,id7)  = K(id7,id7)  + klocal(7,7);
    K(id7,id8)  = K(id7,id8)  + klocal(7,8);
    K(id7,id9)  = K(id7,id9)  + klocal(7,9);
    K(id7,id10) = K(id7,id10) + klocal(7,10);
    K(id7,id11) = K(id7,id11) + klocal(7,11);
    K(id7,id12) = K(id7,id12) + klocal(7,12);
    
    K(id8,id1)  = K(id8,id1)  + klocal(8,1);
    K(id8,id2)  = K(id8,id2)  + klocal(8,2);
    K(id8,id3)  = K(id8,id3)  + klocal(8,3);
    K(id8,id4)  = K(id8,id4)  + klocal(8,4);
    K(id8,id5)  = K(id8,id5)  + klocal(8,5);
    K(id8,id6)  = K(id8,id6)  + klocal(8,6);
    K(id8,id7)  = K(id8,id7)  + klocal(8,7);
    K(id8,id8)  = K(id8,id8)  + klocal(8,8);
    K(id8,id9)  = K(id8,id9)  + klocal(8,9);
    K(id8,id10) = K(id8,id10) + klocal(8,10);
    K(id8,id11) = K(id8,id11) + klocal(8,11);
    K(id8,id12) = K(id8,id12) + klocal(8,12);
    
    K(id9,id1)  = K(id9,id1)  + klocal(9,1);
    K(id9,id2)  = K(id9,id2)  + klocal(9,2);
    K(id9,id3)  = K(id9,id3)  + klocal(9,3);
    K(id9,id4)  = K(id9,id4)  + klocal(9,4);
    K(id9,id5)  = K(id9,id5)  + klocal(9,5);
    K(id9,id6)  = K(id9,id6)  + klocal(9,6);
    K(id9,id7)  = K(id9,id7)  + klocal(9,7);
    K(id9,id8)  = K(id9,id8)  + klocal(9,8);
    K(id9,id9)  = K(id9,id9)  + klocal(9,9);
    K(id9,id10) = K(id9,id10) + klocal(9,10);
    K(id9,id11) = K(id9,id11) + klocal(9,11);
    K(id9,id12) = K(id9,id12) + klocal(9,12);
    
    K(id10,id1)  = K(id10,id1)  + klocal(10,1);
    K(id10,id2)  = K(id10,id2)  + klocal(10,2);
    K(id10,id3)  = K(id10,id3)  + klocal(10,3);
    K(id10,id4)  = K(id10,id4)  + klocal(10,4);
    K(id10,id5)  = K(id10,id5)  + klocal(10,5);
    K(id10,id6)  = K(id10,id6)  + klocal(10,6);
    K(id10,id7)  = K(id10,id7)  + klocal(10,7);
    K(id10,id8)  = K(id10,id8)  + klocal(10,8);
    K(id10,id9)  = K(id10,id9)  + klocal(10,9);
    K(id10,id10) = K(id10,id10) + klocal(10,10);
    K(id10,id11) = K(id10,id11) + klocal(10,11);
    K(id10,id12) = K(id10,id12) + klocal(10,12);
    
    K(id11,id1)  = K(id11,id1)  + klocal(11,1);
    K(id11,id2)  = K(id11,id2)  + klocal(11,2);
    K(id11,id3)  = K(id11,id3)  + klocal(11,3);
    K(id11,id4)  = K(id11,id4)  + klocal(11,4);
    K(id11,id5)  = K(id11,id5)  + klocal(11,5);
    K(id11,id6)  = K(id11,id6)  + klocal(11,6);
    K(id11,id7)  = K(id11,id7)  + klocal(11,7);
    K(id11,id8)  = K(id11,id8)  + klocal(11,8);
    K(id11,id9)  = K(id11,id9)  + klocal(11,9);
    K(id11,id10) = K(id11,id10) + klocal(11,10);
    K(id11,id11) = K(id11,id11) + klocal(11,11);
    K(id11,id12) = K(id11,id12) + klocal(11,12);
    
    K(id12,id1)  = K(id12,id1)  + klocal(12,1);
    K(id12,id2)  = K(id12,id2)  + klocal(12,2);
    K(id12,id3)  = K(id12,id3)  + klocal(12,3);
    K(id12,id4)  = K(id12,id4)  + klocal(12,4);
    K(id12,id5)  = K(id12,id5)  + klocal(12,5);
    K(id12,id6)  = K(id12,id6)  + klocal(12,6);
    K(id12,id7)  = K(id12,id7)  + klocal(12,7);
    K(id12,id8)  = K(id12,id8)  + klocal(12,8);
    K(id12,id9)  = K(id12,id9)  + klocal(12,9);
    K(id12,id10) = K(id12,id10) + klocal(12,10);
    K(id12,id11) = K(id12,id11) + klocal(12,11);
    K(id12,id12) = K(id12,id12) + klocal(12,12);
    
    %-------------------------------------[K] complete
    
end

% Store unlaltered K as Ksing and M as Morig
% before applying the boundary conditions.
Ksing = K;
% Imposing displacement boundary conditions
% ------------------------------------------
% dispID array contains the dof which are assigned specified values.
Funcut = F;
[sm,sn] = size(dispID);
Ndbc = sn;
for nd=1:Ndbc
    for nr=1:6*NNODE-nd+1
        F(nr) = F(nr) - K(nr,dispID(nd)-nd+1) * dispVal(nd);
    end
    K = matcut(K,dispID(nd)-nd+1);
    size(F);
    F = veccut(F,dispID(nd)-nd+1);
end

% To solve for unknown displacements.
% K 
U = inv(K)*F;
% SE = .5*U'*K*U;
C = inv(K);

% Results
% ---------------
% "u" for all nodes (including those where values were specified)
u = zeros(6*NNODE,1);
Cm = zeros(6*NNODE,6*NNODE);
dof = [1:NNODE*6]';
cdof = zeros(NNODE*6,1);
cdof(dispID)=1;
adof=dof(cdof~=1);
pdof=dof(cdof==1);
Funcut(pdof,1)=Ksing(pdof,pdof)*dispVal';

Cm(adof,adof)=C;

for iu=1:Ndbc,
  u(dispID(iu)) = 12345.12345;
end


iuc = 0;
for iu=1:6*NNODE,
    if u(iu) == 12345.12345
     iuc = iuc+1;
  else
     u(iu) = U(iu-iuc);
  end
end

for iu=1:Ndbc,
  u(dispID(iu)) = dispVal(iu);
end


%----------------------------------------------
% Computation of reactions at constrained nodes
%----------------------------------------------
R = Ksing*u;
Rdisp = zeros(1,Ndbc);
for iu=1:Ndbc,
  Rdisp(iu) = R(dispID(iu));
end

%-------------------------------
% Strain energy computation
%-------------------------------
SE = .5*u'*Ksing*u;


%Evaluate Load Flow
% FTransfer=zeros(18,NELEM);
% MTransfer=zeros(18,NELEM);
Lf = zeros(6,NNODE);
% for k=1:NNODE,
%     if k==inp,
%         Lf(:,k)=Funcut(inp*6-5:inp*5);
%     else if det(Cm(k*6-5:k*6,k*6-5:k*6))~=0,
%             
%             Lf(:,k) = inv(Cm(k*6-5:k*6,k*6-5:k*6))*u(k*6-5:k*6);
%             
%         else
%             Lf(:,k) = -R(k*6-5:k*6);
%         end
%     end
% end
% 
% 
% Cin=Cm(inp*6-5:inp*6,inp*6-5:inp*6);

end

