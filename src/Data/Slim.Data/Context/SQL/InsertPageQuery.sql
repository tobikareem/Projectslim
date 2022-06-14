

-- drop table if exists dbo.RazorPage
create table dbo.RazorPage
(
	Id int identity(1,1) not null,
	PageName varchar(100) not null,
	[Description] varchar(500)	null,	
	[URL] nvarchar(500) null,		
	[Enabled] bit constraint df_razorpage_enabled default ((1)) not null,

	constraint PK_RazorPage primary key clustered (Id)
) 
go

-- drop table if exists dbo.ResourceAction 
create table dbo.ResourceAction 
(
	Id int           identity (1, 1) not null,
	ResourceAction   varchar (100) not null,
	[Description]    varchar (200) null,
	[Enabled]        bit           constraint df_resourceaction_enabled default ((1)) not null,
	CreatedBy   int           not null,
	CreatedDate       datetime      not null,
	ModifiedBy   int           null,
	ModifiedDate       datetime      null,

	constraint PK_ResourceAction primary key clustered (Id)
)
go

-- drop index if exists NC_slim_ResourceAction_ResourceAction_Enabled on dbo.ResourceAction
create nonclustered index NC_slim_ResourceAction_ResourceAction_Enabled on dbo.ResourceAction (ResourceAction, [Enabled])
	with (sort_in_tempdb=on)
go

-- drop index if exists NC_slim_ResourceAction_Enabled on dbo.ResourceAction
create nonclustered index NC_slim_ResourceAction_Enabled on dbo.ResourceAction ([Enabled])
	include (ResourceAction)
	with (sort_in_tempdb=on)
go

--drop table if exists dbo.PageResourceActionMap
create table dbo.RazorPageResourceActionMap
(
	Id int identity(1,1) not null,
	RazorPageId int not null,
	ResourceActionId int not null,

	constraint PK_PageResourceActionID primary key clustered (Id),
	constraint FK_RazorPageResourceActionMap_PageId foreign key (RazorPageId) references dbo.RazorPage(Id),
	constraint FK_RazorPageResourceActionMap_ResourceActionId foreign key (ResourceActionId) references dbo.ResourceAction(Id),
) 
go

-- drop index if exists NC_slim_PageResourceActionMap_PageID on dbo.PageResourceActionMap
create nonclustered index NC_slim_PageResourceActionMap_PageID on dbo.RazorPageResourceActionMap (RazorPageId)
	include (ResourceActionId)
	with (sort_in_tempdb=on)
go

-- drop index if exists NC_slim_PageResourceActionMap_ResourceActionID on dbo.PageResourceActionMap
create nonclustered index NC_slim_PageResourceActionMap_ResourceActionID on dbo.RazorPageResourceActionMap (ResourceActionId)
	include (RazorPageId)
	with (sort_in_tempdb=on)
go


-- drop table if exists dbo.PageImage
create table dbo.PageImage
(
	Id int identity(1,1) not null,
	PageImageName varchar(100) not null,
	[Description] varchar(500)	null,	
	ActualImage VARBINARY(MAX) not null,		
	[Enabled]        bit           constraint df_pageImage_enabled default ((1)) not null,
	CreatedBy   int           not null,
	CreatedDate       datetime      not null,
	ModifiedBy   int           null,
	ModifiedDate       datetime      null,

	constraint PK_PageImage primary key clustered (Id)
) 
go


create table dbo.PageSection
(
	Id int identity(1,1) not null,
	RazorPageId int not null,
	PageSectionName varchar(100) not null,
	[Description] varchar(500)	null,	
	
	[Enabled]        bit           constraint df_pageSection_enabled default ((1)) not null,
	CreatedBy   int           not null,
	CreatedDate       datetime      not null,
	ModifiedBy   int           null,
	ModifiedDate       datetime      null,

	constraint PK_PageSection primary key clustered (Id),
	constraint FK_PageSection_PageId foreign key (RazorPageId) references dbo.RazorPage(Id)
) 
go

create table dbo.PageSectionResource
(
	Id int identity(1,1) not null,
	RazorPageId int not null,
	RazorPageSectionId int not null,
	ResourceActionId int not null,

	constraint PK_PageSectionResource primary key clustered (Id),
	constraint FK_PageSectionResource_PageId foreign key (RazorPageId) references dbo.RazorPage(Id),
	constraint FK_PageSectionResource_SectionId foreign key (RazorPageSectionId) references dbo.PageSection(Id),				
	constraint FK_PageSectionResource_ResourceActionId foreign key (ResourceActionId) references dbo.ResourceAction(Id)
) 
go


create table dbo.PageSectionImage
(
	Id int identity(1,1) not null,
	RazorPageId int not null,
	RazorPageSectionId int not null,
	PageImageId int not null,

	constraint PK_PageSectionImage primary key clustered (Id),
	constraint FK_PageSectionImage_PageId foreign key (RazorPageId) references dbo.RazorPage(Id),
	constraint FK_PageSectionImage_SectionId foreign key (RazorPageSectionId) references dbo.PageSection(Id),				
	constraint FK_PageSectionImage_PageImageId foreign key (PageImageId) references dbo.PageImage(Id)
) 
go
