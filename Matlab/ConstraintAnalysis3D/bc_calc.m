function [ BC1, BC2, BC3, BC4, BC5, BC6, BC7, BC8, BC9 ] = bc_calc( T,D,Fr,B,R,L,NNODE )

%Summary of this function goes here
% this function takes the nodes on all faces and calculates the BC matrix
% for all 9 cases
%   Detailed explanation goes here
%case 1
BC1=NaN*ones(NNODE,6);
BC1(R(:),1) = 1; %x
BC1(L(:),1) = 0; %x
BC1(Fr(:),2) = 0; %y
BC1(B(:),2) = 0; %y
BC1(T(:),3) = 0; %z
BC1(D(:),3) = 0; %z
BC1(R(:),5) = 0; %ytheta
BC1(R(:),6) = 0; %ztheta
BC1(L(:),5) = 0; %ytheta
BC1(L(:),6) = 0; %ztheta
BC1(Fr(:),4) = 0; %xtheta
BC1(Fr(:),6) = 0; %ztheta
BC1(B(:),4) = 0; %xtheta
BC1(B(:),6) = 0; %ztheta
BC1(T(:),4) = 0; %xtheta
BC1(T(:),5) = 0; %ytheta
BC1(D(:),4) = 0; %xtheta
BC1(D(:),5) = 0; %ytheta

%case 2
BC2=NaN*ones(NNODE,6);
BC2(R(:),1) = 0; %x
BC2(L(:),1) = 0; %x
BC2(Fr(:),2) = 0; %y
BC2(B(:),2) = 1; %y
BC2(T(:),3) = 0; %z
BC2(D(:),3) = 0; %z
BC2(R(:),5) = 0; %ytheta
BC2(R(:),6) = 0; %ztheta
BC2(L(:),5) = 0; %ytheta
BC2(L(:),6) = 0; %ztheta
BC2(Fr(:),4) = 0; %xtheta
BC2(Fr(:),6) = 0; %ztheta
BC2(B(:),4) = 0; %xtheta
BC2(B(:),6) = 0; %ztheta
BC2(T(:),4) = 0; %xtheta
BC2(T(:),5) = 0; %ytheta
BC2(D(:),4) = 0; %xtheta
BC2(D(:),5) = 0; %ytheta

%case 3
BC3=NaN*ones(NNODE,6);
BC3(R(:),1) = 0; %x
BC3(L(:),1) = 0; %x
BC3(Fr(:),2) = 0; %y
BC3(B(:),2) = 0; %y
BC3(T(:),3) = 1; %z
BC3(D(:),3) = 0; %z
BC3(R(:),5) = 0; %ytheta
BC3(R(:),6) = 0; %ztheta
BC3(L(:),5) = 0; %ytheta
BC3(L(:),6) = 0; %ztheta
BC3(Fr(:),4) = 0; %xtheta
BC3(Fr(:),6) = 0; %ztheta
BC3(B(:),4) = 0; %xtheta
BC3(B(:),6) = 0; %ztheta
BC3(T(:),4) = 0; %xtheta
BC3(T(:),5) = 0; %ytheta
BC3(D(:),4) = 0; %xtheta
BC3(D(:),5) = 0; %ytheta

%case 4
BC4=NaN*ones(NNODE,6);
BC4(R(:),1) = 0; %x
BC4(L(:),1) = 0; %x
BC4(Fr(:),3) = 0; %y
BC4(B(:),3) = 0.5; %y
BC4(T(:),2) = 0.5; %z
BC4(D(:),2) = 0; %z
BC4(R(:),5) = 0; %ytheta
BC4(R(:),6) = 0; %ztheta
BC4(L(:),5) = 0; %ytheta
BC4(L(:),6) = 0; %ztheta
BC4(Fr(:),4) = 0; %xtheta
BC4(Fr(:),6) = 0; %ztheta
BC4(B(:),4) = 0; %xtheta
BC4(B(:),6) = 0; %ztheta
BC4(T(:),4) = 0; %xtheta
BC4(T(:),5) = 0; %ytheta
BC4(D(:),4) = 0; %xtheta
BC4(D(:),5) = 0; %ytheta

%case 5
BC5=NaN*ones(NNODE,6);
BC5(R(:),3) = 0.5; %x
BC5(L(:),3) = 0; %x
BC5(Fr(:),2) = 0; %y
BC5(B(:),2) = 0; %y
BC5(T(:),1) = 0.5; %z
BC5(D(:),1) = 0; %z
BC5(R(:),5) = 0; %ytheta
BC5(R(:),6) = 0; %ztheta
BC5(L(:),5) = 0; %ytheta
BC5(L(:),6) = 0; %ztheta
BC5(Fr(:),4) = 0; %xtheta
BC5(Fr(:),6) = 0; %ztheta
BC5(B(:),4) = 0; %xtheta
BC5(B(:),6) = 0; %ztheta
BC5(T(:),4) = 0; %xtheta
BC5(T(:),5) = 0; %ytheta
BC5(D(:),4) = 0; %xtheta
BC5(D(:),5) = 0; %ytheta

%case 6
BC6=NaN*ones(NNODE,6);
BC6(R(:),2) = 0.5; %x
BC6(L(:),2) = 0; %x
BC6(Fr(:),1) = 0; %y
BC6(B(:),1) = 0.5; %y
BC6(T(:),3) = 0; %z
BC6(D(:),3) = 0; %z
BC6(R(:),5) = 0; %ytheta
BC6(R(:),6) = 0; %ztheta
BC6(L(:),5) = 0; %ytheta
BC6(L(:),6) = 0; %ztheta
BC6(Fr(:),4) = 0; %xtheta
BC6(Fr(:),6) = 0; %ztheta
BC6(B(:),4) = 0; %xtheta
BC6(B(:),6) = 0; %ztheta
BC6(T(:),4) = 0; %xtheta
BC6(T(:),5) = 0; %ytheta
BC6(D(:),4) = 0; %xtheta
BC6(D(:),5) = 0; %ytheta

%case 7
BC7=NaN*ones(NNODE,6);
BC7(R(:),1) = 1; %x
BC7(L(:),1) = 0; %x
BC7(Fr(:),2) = 0; %y
BC7(B(:),2) = 1; %y
BC7(T(:),3) = 0; %z
BC7(D(:),3) = 0; %z
BC7(R(:),5) = 0; %ytheta
BC7(R(:),6) = 0; %ztheta
BC7(L(:),5) = 0; %ytheta
BC7(L(:),6) = 0; %ztheta
BC7(Fr(:),4) = 0; %xtheta
BC7(Fr(:),6) = 0; %ztheta
BC7(B(:),4) = 0; %xtheta
BC7(B(:),6) = 0; %ztheta
BC7(T(:),4) = 0; %xtheta
BC7(T(:),5) = 0; %ytheta
BC7(D(:),4) = 0; %xtheta
BC7(D(:),5) = 0; %ytheta

%case 8
BC8=NaN*ones(NNODE,6);
BC8(R(:),1) = 1; %x
BC8(L(:),1) = 0; %x
BC8(Fr(:),2) = 0; %y
BC8(B(:),2) = 0; %y
BC8(T(:),3) = 1; %z
BC8(D(:),3) = 0; %z
BC8(R(:),5) = 0; %ytheta
BC8(R(:),6) = 0; %ztheta
BC8(L(:),5) = 0; %ytheta
BC8(L(:),6) = 0; %ztheta
BC8(Fr(:),4) = 0; %xtheta
BC8(Fr(:),6) = 0; %ztheta
BC8(B(:),4) = 0; %xtheta
BC8(B(:),6) = 0; %ztheta
BC8(T(:),4) = 0; %xtheta
BC8(T(:),5) = 0; %ytheta
BC8(D(:),4) = 0; %xtheta
BC8(D(:),5) = 0; %ytheta

%case 9
BC9=NaN*ones(NNODE,6);
BC9(R(:),1) = 0; %x
BC9(L(:),1) = 0; %x
BC9(Fr(:),2) = 0; %y
BC9(B(:),2) = 1; %y
BC9(T(:),3) = 1; %z
BC9(D(:),3) = 0; %z
BC9(R(:),5) = 0; %ytheta
BC9(R(:),6) = 0; %ztheta
BC9(L(:),5) = 0; %ytheta
BC9(L(:),6) = 0; %ztheta
BC9(Fr(:),4) = 0; %xtheta
BC9(Fr(:),6) = 0; %ztheta
BC9(B(:),4) = 0; %xtheta
BC9(B(:),6) = 0; %ztheta
BC9(T(:),4) = 0; %xtheta
BC9(T(:),5) = 0; %ytheta
BC9(D(:),4) = 0; %xtheta
BC9(D(:),5) = 0; %ytheta

end

