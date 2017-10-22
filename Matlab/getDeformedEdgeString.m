function finalString = getDeformedEdgeString(origEdgeString, nnode, nmesh)
    nmesh = nmesh - 1; %there are n-1 intermediate points for n intervals
    finalString = origEdgeString;
    intervalIndex = 1;
    currNodeIndex = 1;
    currMeshIndex = 1 + nnode;
    nodeStrings = strsplit(origEdgeString, ' ');
    finalString = strcat(finalString, {'|'});
    for i = 1:nnode - 1
        finalString = strcat(finalString, nodeStrings(currNodeIndex));
        finalString = strcat(finalString, {' '});
        finalString = strcat(finalString, num2str(currMeshIndex));
        finalString = strcat(finalString, {' '});
        startIndex = (1 + nnode + (nmesh * (intervalIndex - 1))+ 1);
        endIndex = (1 + nnode + (nmesh * intervalIndex) - 1);
        for j = startIndex:endIndex
            finalString = strcat(finalString, num2str(j));
            finalString = strcat(finalString, {' '});            
        end
        finalString = strcat(finalString, nodeStrings(currNodeIndex + 1));
        finalString = strcat(finalString, {';'});
        currMeshIndex = 1 + nnode + (nmesh * intervalIndex);
        intervalIndex = intervalIndex + 1;
        currNodeIndex = currNodeIndex + 2;
    end
    finalString = finalString{1};
end