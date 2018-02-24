function handles = Design1_a(X,handles)
handles.node=[1 0.5 0.0 0.0;
    2 0.0 0.5 0.0;
    3 0.0 0.5 0.5;];
handles.elem=[1 1 2 0.01 0.01 200e3 2;
    2 1 3 0.01 0.01 200e3 2;
    3 2 3 0.01 0.01 200e3 2;];
handles.T = [3];% pos z 
handles.D = [1 2 4];
handles.Fr = [1];
handles.B = [2 3 6];% pos y 
handles.R = [1];% pos x 
handles.L = [2 3 6];

% handles = bin_categorize(handles);
end