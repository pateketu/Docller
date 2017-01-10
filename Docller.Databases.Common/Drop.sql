declare @procName varchar(500)
declare cur cursor 

for select [name] from sys.objects where type = 'P'
open cur
fetch next from cur into @procName
while @@fetch_status = 0
begin
    exec('drop proc ' + @procName)
    fetch next from cur into @procName
end
close cur
deallocate cur


declare @procName varchar(500)
declare cur cursor 

for select [name] from sys.objects where type = 'U'
open cur
fetch next from cur into @procName
while @@fetch_status = 0
begin
    exec('drop table ' + @procName)
    fetch next from cur into @procName
end
close cur
deallocate cur