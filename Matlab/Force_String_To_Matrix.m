
A = strsplit(forceString, ';');

% Method 1

A='55 3@ 4 5@ 47 89@ 33 12@'
out = reshape(str2double(regexp(A,'\d*','match')),2,[])







% Method 2: 4x2 Matrix

A='55 3@ 4 5@ 47 89@ 33 12@';
B = strrep(A,'@',' ');
C = char(strsplit(B));
D = reshape(str2num(C), 2, [])';





% Method 3: Different number of columns in each row


A='5 1 2 2 3@4 3 3 4 5@4 7 8 7 9@4 44 3 3 12@';
A='5 5 3@ 4 5 5@ 4 7 89@ 3 8 12@';

%at = strfind(A,'@');

%ats = length(at);

atn = strsplit(A,'@');

%a = strsplit(atn{1});

satn = length(atn);
B = strrep(A,'@',' ');
C = char(strsplit(B));
D = reshape(str2num(C), satn, [])';






% Method 4:

A='55 3@ 4 5@ 47 89@ 33 12@' % Create sample data.
intA = sscanf(A, '%d %d@')   % Make string into numerical array.
s=reshape(intA, [numel(intA)/2, 2])  % Shape into two column matrix.




% Method 5: Irregular Number of Columns


clc;
A='55 3@ 4 5@ 47 89@ 33 12@' % Create sample data.
atLocation = find(A=='@');
numNumbers = 1+sum(A(1:atLocation)==' ')
out = reshape(str2double(regexp(A,'\d*','match')), numNumbers,[])'
A='5 5 3@ 4 5 5@ 4 7 89@ 3 8 12@'
atLocation = find(A=='@');
numNumbers = 1+sum(A(1:atLocation)==' ')
out = reshape(str2double(regexp(A,'\d*','match')), numNumbers,[])'
A='5 1 2 2 3@4 3 3 4 5@4 7 8 7 9@4 44 3 3 12@'
atLocation = find(A=='@');
numNumbers = 1+sum(A(1:atLocation)==' ')
out = reshape(str2double(regexp(A,'\d*','match')), numNumbers,[])'




% Method 6: Irregular Number of Columns with spaces

A='79 197 @ 80 197 @ 81 198 @ 82 198 @ 83 199 @';
atLocation = find(A=='@');
trimmedString = strtrim(A(1:atLocation-1))
numNumbers = 1+sum(trimmedString==' ')
out = reshape(str2double(regexp(A,'\d*','match')), numNumbers,[])'
