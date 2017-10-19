function finalString = getDeformedEdgeString(origEdgeString, nnode, nmesh)
    nmesh = nmesh - 1; %there are n-1 intermediate points for n intervals
    finalString = origEdgeString;
    currNodeIndex = 1;
    currMeshIndex = 1 + nnode;
    finalString = strcat(finalString, {' '});
    for i = 1:nnode - 1
        finalString = strcat(finalString, num2str(currNodeIndex));
        finalString = strcat(finalString, {' '});
        finalString = strcat(finalString, num2str(currMeshIndex));
        finalString = strcat(finalString, {' '});
        startIndex = (1 + nnode + (nmesh * (currNodeIndex - 1))+ 1);
        endIndex = (1 + nnode + (nmesh * currNodeIndex) - 1);
        for j = startIndex:endIndex
            finalString = strcat(finalString, num2str(j-1));
            finalString = strcat(finalString, {' '});
            finalString = strcat(finalString, num2str(j));
            finalString = strcat(finalString, {' '});            
        end
        finalString = strcat(finalString, num2str(j));
        finalString = strcat(finalString, {' '});
        finalString = strcat(finalString, num2str(currNodeIndex + 1));
        finalString = strcat(finalString, {' '});
        currMeshIndex = 1 + nnode + (nmesh * currNodeIndex);
        currNodeIndex = currNodeIndex + 1;
    end
    finalString = finalString{1};
end