function finalString = getDeformedCoordString(node, nnode)
    finalString = '';
    for i = 1:size(node,1)
        finalString = strcat(finalString, num2str(node(i,1)));
        finalString = strcat(finalString,'(');
        for j = 2:size(node,2)
            tempStr = num2str(node(i,j), '%.4f');
            finalString = strcat(finalString,tempStr);
            if(j ~= size(node,2))
                finalString = strcat(finalString, {', '});
            end
            
        end
        finalString = strcat(finalString, ');');
        if i == nnode
            finalString = strcat(finalString, '|');
        end
    end
    finalString = finalString{1};
end