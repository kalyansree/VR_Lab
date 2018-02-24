function [ temp_T, temp_D, temp_F, temp_B, temp_R, temp_L ] = dof_calc( T,D,F,B,R,L )
%dof_calc Summary of this function goes here
% This function calculates the dof's of each face 
%   Detailed explanation goes here
%  inputs : nodes for each face T(top), D(down) , F(front), B(back),
%  R(right), L(left) 
% Output : vector temp_T (n*6) matrix with each row containing all the six
% dof of each node

count  = 1;
%initialize all return values in case the inputs are empty 
temp_T = [];
temp_D = [];
temp_F = [];
temp_B = [];
temp_R = [];
temp_L = [];
for i = [T] % symmetry plane is XY and z-disp and xtheta,ytheta must be zero
    temp_T(count,:) = [6*i-5 6*i-5+1 6*i-5+2 6*i-5+3 6*i-5+4 6*i-5+5]; 
    count = count + 1;
end

count  = 1;
for i = [D] % symmetry plane is XY and z-disp and xtheta,ytheta must be zero
    temp_D(count,:) = [6*i-5 6*i-5+1 6*i-5+2 6*i-5+3 6*i-5+4 6*i-5+5];
    count = count + 1;
end

count  = 1;
for i = [F] % symmetry plane is XY and z-disp and xtheta,ytheta must be zero
    temp_F(count,:) = [6*i-5 6*i-5+1 6*i-5+2 6*i-5+3 6*i-5+4 6*i-5+5];
    count = count + 1;
end

count  = 1;
for i = [B] % symmetry plane is XY and z-disp and xtheta,ytheta must be zero
    temp_B(count,:) = [6*i-5 6*i-5+1 6*i-5+2 6*i-5+3 6*i-5+4 6*i-5+5];
    count = count + 1;
end

count  = 1;
for i = [R] % symmetry plane is XY and z-disp and xtheta,ytheta must be zero
    temp_R(count,:) = [6*i-5 6*i-5+1 6*i-5+2 6*i-5+3 6*i-5+4 6*i-5+5];
    count = count + 1;
end

count  = 1;
for i = [L] % symmetry plane is XY and z-disp and xtheta,ytheta must be zero
    temp_L(count,:) = [6*i-5 6*i-5+1 6*i-5+2 6*i-5+3 6*i-5+4 6*i-5+5];
    count = count + 1;
end


end

