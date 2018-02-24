function handles = bin_categorize(handles)
X = handles.node(:,2);
Y = handles.node(:,3);
Z = handles.node(:,4);
handles.T = transpose(find(Z == 0.5));% pos z 
handles.D = transpose(find(Z == 0));
handles.Fr = transpose(find(Y == 0));
handles.B = transpose(find(Y == 0.5));% pos y 
handles.R = transpose(find(X == 0.5));% pos x 
handles.L = transpose(find(X == 0));
end