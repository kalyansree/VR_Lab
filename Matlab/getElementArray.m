function arr = getElementArray(str,  nmesh)
    n = length(str) / 4;
    arr = zeros(n, 7);
    doubleCell = str2double(strsplit(str, ' '));
    for i=1:n
        arr(i,1) = i;
        arr(i,2:3) = doubleCell(2*i-1:2*i);
        arr(i,4:7) = [0.01, 0.01, 200e3, nmesh];
    end
    
end