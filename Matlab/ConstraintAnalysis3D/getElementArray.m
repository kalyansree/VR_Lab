function arr = getElementArray(str,  nmesh)
    doubleCell = str2double(strsplit(str, ' '));
    length(doubleCell)
    n = (length(doubleCell) - 1) / 2;
    arr = zeros(n, 7);
    for i=1:n
        arr(i,1) = i;
        arr(i,2:3) = doubleCell(2*i-1:2*i);
        arr(i,4:7) = [0.01, 0.01, 200e3, nmesh];
    end
    
end