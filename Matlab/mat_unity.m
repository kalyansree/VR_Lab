function [ u ] = mat_unity( node, Inp, Oup, Fix, T, elem )

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

count  = 1;
% T = [ 1 2 3 4 5 6 7 8 9 10]; 
% for i = [T] % symmetry plane is XY and z-disp and xtheta,ytheta must be zero
%     temp_T(count,:) = [6*i-5 6*i-5+1 6*i-5+2 6*i-5+3 6*i-5+4 6*i-5+5]; 
%     count = count + 1;
% end

T = [ 1 2 3 4 5 6]; 
for i = [T] % symmetry plane is XY and z-disp and xtheta,ytheta must be zero
    temp_T(count,:) = [6*i-5 6*i-5+1 6*i-5+2 6*i-5+3 6*i-5+4 6*i-5+5]; 
    count = count + 1;
end

% dispID_1 = [temp_T(1,:) temp_T(5,:) temp_T(7,:) temp_T(8,:) temp_T(2,1) temp_T(2,3)];
% dispVal_1 = [zeros(1,6) zeros(1,6) zeros(1,6) zeros(1,6) 1 0];

% dispID_1 = [temp_T(1,:) temp_T(5,:) temp_T(6,:) temp_T(2,1) temp_T(2,3)];
% dispVal_1 = [zeros(1,6) zeros(1,6) zeros(1,6) 1 0];

dispID_1 = [reshape(temp_T(Fix,:),1,numel(temp_T(Fix,:))) temp_T(Inp,1)];
dispVal_1 = [zeros(1,numel(temp_T(Fix,:))) 1];

% dispID_1 = [temp_T(1,:) temp_T(5,:) temp_T(6,:) temp_T(9,:) temp_T(10,:) temp_T(2,1) temp_T(2,3)];
% dispVal_1 = [zeros(1,6) zeros(1,6) zeros(1,6) zeros(1,6) zeros(1,6) 1 0];

dispsort1 = sortrows([dispID_1' dispVal_1'],1);% if you don't sort you will have problems while doing F-KU
dispsort_unique1 = unique(dispsort1,'rows');
dispID1 = dispsort_unique1(:,1)';
dispVal1 = dispsort_unique1(:,2)';
% BC1 = [ 0 0 0 0 0 0; 1 NaN 0 NaN NaN NaN; NaN NaN NaN NaN NaN NaN; NaN NaN NaN NaN NaN NaN; ...
%     0 0 0 0 0 0; 0 0 0 0 0 0; NaN NaN NaN NaN NaN NaN];
BC1 = [ 0 0 0 0 0 0; 1 NaN 0 NaN NaN NaN; NaN NaN NaN NaN NaN NaN; NaN NaN NaN NaN NaN NaN; ...
    0 0 0 0 0 0; 0 0 0 0 0 0];
% BC1 = [ 0 0 0 0 0 0; 1 NaN 0 NaN NaN NaN; NaN NaN NaN NaN NaN NaN; NaN NaN NaN NaN NaN NaN; ...
%     0 0 0 0 0 0; NaN NaN NaN NaN NaN NaN;0 0 0 0 0 0;0 0 0 0 0 0];

% BC1 = [ 0 0 0 0 0 0; 1 NaN 0 NaN NaN NaN; NaN NaN NaN NaN NaN NaN; NaN NaN NaN NaN NaN NaN; ...
%     0 0 0 0 0 0;0 0 0 0 0 0; NaN NaN NaN NaN NaN NaN;NaN NaN NaN NaN NaN NaN;0 0 0 0 0 0;0 0 0 0 0 0];


[u1,R1,K1,SE1] = fem_3d(Be,He,Le,Y,t,ncon,NELEM,NNODE,F,dispID1,dispVal1);
dim = 3; 
[Cglobal1,Felem1]=LoadFlow_general(NNODE,NELEM,K1,BC1',R1,dim);

FT1 = Felem1(1:3,:); MT1 = Felem1(4:6,:); 

for i = 1:NNODE
    qu1(i) = FT1(1,i)/norm(FT1(:,i)); 
    qv1(i) = FT1(2,i)/norm(FT1(:,i)); 
    qw1(i) = FT1(3,i)/norm(FT1(:,i));    
end


for j = 1
    figure(j)
    u = [u1];
    qu = [qu1'];
    qv = [qv1']; 
    qw = [qw1']; 
    for i = 1:NELEM,
        id1 = ncon(i,1);
        id2 = ncon(i,2);
        uid1 = 6*(id1-1) + 1;
        uid2 = 6*(id2-1) + 1;
        disp_scaling = 0.3; 
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


end

