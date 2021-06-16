USE [master]
GO
/****** Object:  Database [vault]    Script Date: 16/06/2021 10:30:53 ******/
CREATE DATABASE [vault]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'vault', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\vault.mdf' , SIZE = 4288KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'vault_log', FILENAME = N'C:\Program Files\Microsoft SQL Server\MSSQL12.MSSQLSERVER\MSSQL\DATA\vault_log.ldf' , SIZE = 1856KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [vault].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [vault] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [vault] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [vault] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [vault] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [vault] SET ARITHABORT OFF 
GO
ALTER DATABASE [vault] SET AUTO_CLOSE OFF 
GO
ALTER DATABASE [vault] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [vault] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [vault] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [vault] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [vault] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [vault] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [vault] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [vault] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [vault] SET  ENABLE_BROKER 
GO
ALTER DATABASE [vault] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [vault] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [vault] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [vault] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [vault] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [vault] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [vault] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [vault] SET RECOVERY FULL 
GO
ALTER DATABASE [vault] SET  MULTI_USER 
GO
ALTER DATABASE [vault] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [vault] SET DB_CHAINING OFF 
GO
ALTER DATABASE [vault] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [vault] SET TARGET_RECOVERY_TIME = 60 SECONDS 
GO
ALTER DATABASE [vault] SET DELAYED_DURABILITY = DISABLED 
GO
EXEC sys.sp_db_vardecimal_storage_format N'vault', N'ON'
GO
USE [vault]
GO
/****** Object:  UserDefinedTableType [dbo].[GuidIdArray]    Script Date: 16/06/2021 10:30:53 ******/
CREATE TYPE [dbo].[GuidIdArray] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL
)
GO
/****** Object:  UserDefinedTableType [dbo].[SelectionReadOnlyArray]    Script Date: 16/06/2021 10:30:53 ******/
CREATE TYPE [dbo].[SelectionReadOnlyArray] AS TABLE(
	[Id] [uniqueidentifier] NOT NULL,
	[ReadOnly] [bit] NOT NULL,
	[HidePasswords] [bit] NOT NULL
)
GO
/****** Object:  Table [dbo].[__EFMigrationsHistory]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[__EFMigrationsHistory](
	[MigrationId] [nvarchar](150) NOT NULL,
	[ProductVersion] [nvarchar](32) NOT NULL,
 CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY CLUSTERED 
(
	[MigrationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Cipher]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Cipher](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[OrganizationId] [uniqueidentifier] NULL,
	[Type] [tinyint] NOT NULL,
	[Data] [nvarchar](max) NOT NULL,
	[Favorites] [nvarchar](max) NULL,
	[Folders] [nvarchar](max) NULL,
	[Attachments] [nvarchar](max) NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[RevisionDate] [datetime2](7) NOT NULL,
	[DeletedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_Cipher] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Collection]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Collection](
	[Id] [uniqueidentifier] NOT NULL,
	[OrganizationId] [uniqueidentifier] NOT NULL,
	[Name] [varchar](max) NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[RevisionDate] [datetime2](7) NOT NULL,
	[ExternalId] [nvarchar](300) NULL,
 CONSTRAINT [PK_Collection] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[CollectionCipher]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CollectionCipher](
	[CollectionId] [uniqueidentifier] NOT NULL,
	[CipherId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_CollectionCipher] PRIMARY KEY CLUSTERED 
(
	[CollectionId] ASC,
	[CipherId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CollectionGroup]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CollectionGroup](
	[CollectionId] [uniqueidentifier] NOT NULL,
	[GroupId] [uniqueidentifier] NOT NULL,
	[ReadOnly] [bit] NOT NULL,
	[HidePasswords] [bit] NOT NULL,
 CONSTRAINT [PK_CollectionGroup] PRIMARY KEY CLUSTERED 
(
	[CollectionId] ASC,
	[GroupId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CollectionUser]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CollectionUser](
	[CollectionId] [uniqueidentifier] NOT NULL,
	[OrganizationUserId] [uniqueidentifier] NOT NULL,
	[ReadOnly] [bit] NOT NULL,
	[HidePasswords] [bit] NOT NULL,
 CONSTRAINT [PK_CollectionUser] PRIMARY KEY CLUSTERED 
(
	[CollectionId] ASC,
	[OrganizationUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Device]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Device](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[Type] [smallint] NOT NULL,
	[Identifier] [nvarchar](50) NOT NULL,
	[PushToken] [nvarchar](255) NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[RevisionDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Device] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[EmergencyAccess]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[EmergencyAccess](
	[Id] [uniqueidentifier] NOT NULL,
	[GrantorId] [uniqueidentifier] NOT NULL,
	[GranteeId] [uniqueidentifier] NULL,
	[Email] [nvarchar](50) NULL,
	[KeyEncrypted] [varchar](max) NULL,
	[WaitTimeDays] [smallint] NULL,
	[Type] [tinyint] NOT NULL,
	[Status] [tinyint] NOT NULL,
	[RecoveryInitiatedDate] [datetime2](7) NULL,
	[LastNotificationDate] [datetime2](7) NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[RevisionDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_EmergencyAccess] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Event]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Event](
	[Id] [uniqueidentifier] NOT NULL,
	[Type] [int] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[OrganizationId] [uniqueidentifier] NULL,
	[CipherId] [uniqueidentifier] NULL,
	[CollectionId] [uniqueidentifier] NULL,
	[GroupId] [uniqueidentifier] NULL,
	[OrganizationUserId] [uniqueidentifier] NULL,
	[ActingUserId] [uniqueidentifier] NULL,
	[DeviceType] [smallint] NULL,
	[IpAddress] [varchar](50) NULL,
	[Date] [datetime2](7) NOT NULL,
	[PolicyId] [uniqueidentifier] NULL,
 CONSTRAINT [PK_Event] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Folder]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Folder](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[Name] [varchar](max) NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[RevisionDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Folder] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Grant]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Grant](
	[Key] [nvarchar](200) NOT NULL,
	[Type] [nvarchar](50) NOT NULL,
	[SubjectId] [nvarchar](200) NULL,
	[ClientId] [nvarchar](200) NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[ExpirationDate] [datetime2](7) NULL,
	[Data] [nvarchar](max) NOT NULL,
	[SessionId] [nvarchar](100) NULL,
	[Description] [nvarchar](200) NULL,
	[ConsumedDate] [datetime2](7) NULL,
 CONSTRAINT [PK_Grant] PRIMARY KEY CLUSTERED 
(
	[Key] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Group]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Group](
	[Id] [uniqueidentifier] NOT NULL,
	[OrganizationId] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[AccessAll] [bit] NOT NULL,
	[ExternalId] [nvarchar](300) NULL,
	[CreationDate] [datetime] NOT NULL,
	[RevisionDate] [datetime] NOT NULL,
 CONSTRAINT [PK_Group] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[GroupUser]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[GroupUser](
	[GroupId] [uniqueidentifier] NOT NULL,
	[OrganizationUserId] [uniqueidentifier] NOT NULL,
 CONSTRAINT [PK_GroupUser] PRIMARY KEY CLUSTERED 
(
	[GroupId] ASC,
	[OrganizationUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Installation]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Installation](
	[Id] [uniqueidentifier] NOT NULL,
	[Email] [nvarchar](50) NOT NULL,
	[Key] [varchar](150) NOT NULL,
	[Enabled] [bit] NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Installation] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Migration]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Migration](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ScriptName] [nvarchar](255) NOT NULL,
	[Applied] [datetime] NOT NULL,
 CONSTRAINT [PK_Migration_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Organization]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Organization](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NOT NULL,
	[BusinessName] [nvarchar](50) NULL,
	[BillingEmail] [nvarchar](50) NOT NULL,
	[Plan] [nvarchar](50) NOT NULL,
	[PlanType] [tinyint] NOT NULL,
	[Seats] [smallint] NULL,
	[MaxCollections] [smallint] NULL,
	[UseGroups] [bit] NOT NULL,
	[UseDirectory] [bit] NOT NULL,
	[UseTotp] [bit] NOT NULL,
	[SelfHost] [bit] NOT NULL,
	[Storage] [bigint] NULL,
	[MaxStorageGb] [smallint] NULL,
	[Gateway] [tinyint] NULL,
	[GatewayCustomerId] [varchar](50) NULL,
	[GatewaySubscriptionId] [varchar](50) NULL,
	[Enabled] [bit] NOT NULL,
	[LicenseKey] [varchar](100) NULL,
	[ExpirationDate] [datetime2](7) NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[RevisionDate] [datetime2](7) NOT NULL,
	[BusinessAddress1] [nvarchar](50) NULL,
	[BusinessAddress2] [nvarchar](50) NULL,
	[BusinessAddress3] [nvarchar](50) NULL,
	[BusinessCountry] [varchar](2) NULL,
	[BusinessTaxNumber] [nvarchar](30) NULL,
	[UsersGetPremium] [bit] NOT NULL,
	[UseEvents] [bit] NOT NULL,
	[Use2fa] [bit] NOT NULL,
	[TwoFactorProviders] [nvarchar](max) NULL,
	[UseApi] [bit] NOT NULL,
	[ApiKey] [varchar](30) NOT NULL,
	[UsePolicies] [bit] NOT NULL,
	[Identifier] [nvarchar](50) NULL,
	[ReferenceData] [nvarchar](max) NULL,
	[UseSso] [bit] NOT NULL,
 CONSTRAINT [PK_Organization] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[OrganizationUser]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[OrganizationUser](
	[Id] [uniqueidentifier] NOT NULL,
	[OrganizationId] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[Email] [nvarchar](50) NULL,
	[Key] [varchar](max) NULL,
	[Status] [tinyint] NOT NULL,
	[Type] [tinyint] NOT NULL,
	[AccessAll] [bit] NOT NULL,
	[ExternalId] [nvarchar](300) NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[RevisionDate] [datetime2](7) NOT NULL,
	[Permissions] [nvarchar](max) NULL,
 CONSTRAINT [PK_OrganizationUser] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Policy]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Policy](
	[Id] [uniqueidentifier] NOT NULL,
	[OrganizationId] [uniqueidentifier] NOT NULL,
	[Type] [tinyint] NOT NULL,
	[Data] [nvarchar](max) NULL,
	[Enabled] [bit] NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[RevisionDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Policy] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Send]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Send](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[OrganizationId] [uniqueidentifier] NULL,
	[Type] [tinyint] NOT NULL,
	[Data] [varchar](max) NOT NULL,
	[Key] [varchar](max) NOT NULL,
	[Password] [nvarchar](300) NULL,
	[MaxAccessCount] [int] NULL,
	[AccessCount] [int] NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[RevisionDate] [datetime2](7) NOT NULL,
	[ExpirationDate] [datetime2](7) NULL,
	[DeletionDate] [datetime2](7) NOT NULL,
	[Disabled] [bit] NOT NULL,
 CONSTRAINT [PK_Send] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[SsoConfig]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SsoConfig](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[Enabled] [bit] NOT NULL,
	[OrganizationId] [uniqueidentifier] NOT NULL,
	[Data] [nvarchar](max) NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[RevisionDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_SsoConfig] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SsoUser]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SsoUser](
	[Id] [bigint] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[OrganizationId] [uniqueidentifier] NULL,
	[ExternalId] [nvarchar](50) NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_SsoUser] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TaxRate]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[TaxRate](
	[Id] [varchar](40) NOT NULL,
	[Country] [varchar](50) NOT NULL,
	[State] [varchar](2) NULL,
	[PostalCode] [varchar](10) NOT NULL,
	[Rate] [decimal](5, 2) NOT NULL,
	[Active] [bit] NOT NULL,
 CONSTRAINT [PK_TaxRate] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[Transaction]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Transaction](
	[Id] [uniqueidentifier] NOT NULL,
	[UserId] [uniqueidentifier] NULL,
	[OrganizationId] [uniqueidentifier] NULL,
	[Type] [tinyint] NOT NULL,
	[Amount] [money] NOT NULL,
	[Refunded] [bit] NULL,
	[RefundedAmount] [money] NULL,
	[Details] [nvarchar](100) NULL,
	[PaymentMethodType] [tinyint] NULL,
	[Gateway] [tinyint] NULL,
	[GatewayId] [varchar](50) NULL,
	[CreationDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_Transaction] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[U2f]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[U2f](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [uniqueidentifier] NOT NULL,
	[KeyHandle] [varchar](200) NULL,
	[Challenge] [varchar](200) NOT NULL,
	[AppId] [varchar](50) NOT NULL,
	[Version] [varchar](20) NOT NULL,
	[CreationDate] [datetime2](7) NOT NULL,
 CONSTRAINT [PK_U2f] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[User]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[User](
	[Id] [uniqueidentifier] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[Email] [nvarchar](50) NOT NULL,
	[EmailVerified] [bit] NOT NULL,
	[MasterPassword] [nvarchar](300) NULL,
	[MasterPasswordHint] [nvarchar](50) NULL,
	[Culture] [nvarchar](10) NOT NULL,
	[SecurityStamp] [nvarchar](50) NOT NULL,
	[TwoFactorProviders] [nvarchar](max) NULL,
	[TwoFactorRecoveryCode] [nvarchar](32) NULL,
	[EquivalentDomains] [nvarchar](max) NULL,
	[ExcludedGlobalEquivalentDomains] [nvarchar](max) NULL,
	[AccountRevisionDate] [datetime2](7) NOT NULL,
	[Key] [varchar](max) NULL,
	[PublicKey] [varchar](max) NULL,
	[PrivateKey] [varchar](max) NULL,
	[Premium] [bit] NOT NULL,
	[PremiumExpirationDate] [datetime2](7) NULL,
	[Storage] [bigint] NULL,
	[MaxStorageGb] [smallint] NULL,
	[Gateway] [tinyint] NULL,
	[GatewayCustomerId] [varchar](50) NULL,
	[GatewaySubscriptionId] [varchar](50) NULL,
	[LicenseKey] [varchar](100) NULL,
	[CreationDate] [datetime2](7) NOT NULL,
	[RevisionDate] [datetime2](7) NOT NULL,
	[RenewalReminderDate] [datetime2](7) NULL,
	[Kdf] [tinyint] NOT NULL,
	[KdfIterations] [int] NOT NULL,
	[ReferenceData] [nvarchar](max) NULL,
	[ApiKey] [varchar](30) NOT NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  UserDefinedFunction [dbo].[CipherDetails]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[CipherDetails](@UserId UNIQUEIDENTIFIER)
RETURNS TABLE
AS RETURN
SELECT
    C.[Id],
    C.[UserId],
    C.[OrganizationId],
    C.[Type],
    C.[Data],
    C.[Attachments],
    C.[CreationDate],
    C.[RevisionDate],
    C.[Favorite],
    C.[FolderId],
    C.[DeletedDate]
FROM
    [dbo].[Cipher] C
GO
/****** Object:  UserDefinedFunction [dbo].[UserCipherDetails]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[UserCipherDetails](@UserId UNIQUEIDENTIFIER)
RETURNS TABLE
AS RETURN
WITH [CTE] AS (
    SELECT
        [Id],
        [OrganizationId],
        [AccessAll]
    FROM
        [OrganizationUser]
    WHERE
        [UserId] = @UserId
        AND [Status] = 2 -- Confirmed
)
SELECT
    C.*,
    CASE
        WHEN
            OU.[AccessAll] = 1
            OR G.[AccessAll] = 1
            OR COALESCE(CU.[ReadOnly], CG.[ReadOnly], 0) = 0
        THEN 1
        ELSE 0
    END [Edit],
    CASE
        WHEN
            OU.[AccessAll] = 1
            OR G.[AccessAll] = 1
            OR COALESCE(CU.[HidePasswords], CG.[HidePasswords], 0) = 0
        THEN 1
        ELSE 0
    END [ViewPassword],
    CASE
        WHEN O.[UseTotp] = 1
        THEN 1
        ELSE 0
    END [OrganizationUseTotp]
FROM
    [dbo].[CipherDetails](@UserId) C
INNER JOIN
    [CTE] OU ON C.[UserId] IS NULL AND C.[OrganizationId] IN (SELECT [OrganizationId] FROM [CTE])
INNER JOIN
    [dbo].[Organization] O ON O.[Id] = OU.[OrganizationId] AND O.[Id] = C.[OrganizationId] AND O.[Enabled] = 1
LEFT JOIN
    [dbo].[CollectionCipher] CC ON OU.[AccessAll] = 0 AND CC.[CipherId] = C.[Id]
LEFT JOIN
    [dbo].[CollectionUser] CU ON CU.[CollectionId] = CC.[CollectionId] AND CU.[OrganizationUserId] = OU.[Id]
LEFT JOIN
    [dbo].[GroupUser] GU ON CU.[CollectionId] IS NULL AND OU.[AccessAll] = 0 AND GU.[OrganizationUserId] = OU.[Id]
LEFT JOIN
    [dbo].[Group] G ON G.[Id] = GU.[GroupId]
LEFT JOIN
    [dbo].[CollectionGroup] CG ON G.[AccessAll] = 0 AND CG.[CollectionId] = CC.[CollectionId] AND CG.[GroupId] = GU.[GroupId]
WHERE
    OU.[AccessAll] = 1
    OR CU.[CollectionId] IS NOT NULL
    OR G.[AccessAll] = 1
    OR CG.[CollectionId] IS NOT NULL

UNION ALL

SELECT
    *,
    1 [Edit],
    1 [ViewPassword],
    0 [OrganizationUseTotp]
FROM
    [dbo].[CipherDetails](@UserId)
WHERE
    [UserId] = @UserId
GO
/****** Object:  View [dbo].[CollectionView]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CollectionView]
AS
SELECT
    *
FROM
    [dbo].[Collection]
GO
/****** Object:  UserDefinedFunction [dbo].[UserCollectionDetails]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE FUNCTION [dbo].[UserCollectionDetails](@UserId UNIQUEIDENTIFIER)
RETURNS TABLE
AS RETURN
SELECT
    C.*,
    CASE
        WHEN
            OU.[AccessAll] = 1
            OR G.[AccessAll] = 1
            OR COALESCE(CU.[ReadOnly], CG.[ReadOnly], 0) = 0
        THEN 0
        ELSE 1
    END [ReadOnly],
    CASE
        WHEN
            OU.[AccessAll] = 1
            OR G.[AccessAll] = 1
            OR COALESCE(CU.[HidePasswords], CG.[HidePasswords], 0) = 0
        THEN 0
        ELSE 1
    END [HidePasswords]
FROM
    [dbo].[CollectionView] C
INNER JOIN
    [dbo].[OrganizationUser] OU ON C.[OrganizationId] = OU.[OrganizationId]
INNER JOIN
    [dbo].[Organization] O ON O.[Id] = C.[OrganizationId]
LEFT JOIN
    [dbo].[CollectionUser] CU ON OU.[AccessAll] = 0 AND CU.[CollectionId] = C.[Id] AND CU.[OrganizationUserId] = [OU].[Id]
LEFT JOIN
    [dbo].[GroupUser] GU ON CU.[CollectionId] IS NULL AND OU.[AccessAll] = 0 AND GU.[OrganizationUserId] = OU.[Id]
LEFT JOIN
    [dbo].[Group] G ON G.[Id] = GU.[GroupId]
LEFT JOIN
    [dbo].[CollectionGroup] CG ON G.[AccessAll] = 0 AND CG.[CollectionId] = C.[Id] AND CG.[GroupId] = GU.[GroupId]
WHERE
    OU.[UserId] = @UserId
    AND OU.[Status] = 2 -- 2 = Confirmed
    AND O.[Enabled] = 1
    AND (
        OU.[AccessAll] = 1
        OR CU.[CollectionId] IS NOT NULL
        OR G.[AccessAll] = 1
        OR CG.[CollectionId] IS NOT NULL
    )
GO
/****** Object:  View [dbo].[CipherView]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[CipherView]
AS
SELECT
    *
FROM
    [dbo].[Cipher]
GO
/****** Object:  View [dbo].[DeviceView]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[DeviceView]
AS
SELECT
    *
FROM
    [dbo].[Device]
GO
/****** Object:  View [dbo].[EmergencyAccessDetailsView]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[EmergencyAccessDetailsView]
AS
SELECT
    EA.*,
    GranteeU.[Name] GranteeName,
    ISNULL(GranteeU.[Email], EA.[Email]) GranteeEmail,
    GrantorU.[Name] GrantorName,
    GrantorU.[Email] GrantorEmail
FROM
    [dbo].[EmergencyAccess] EA
LEFT JOIN
    [dbo].[User] GranteeU ON GranteeU.[Id] = EA.[GranteeId]
LEFT JOIN
    [dbo].[User] GrantorU ON GrantorU.[Id] = EA.[GrantorId]
GO
/****** Object:  View [dbo].[EventView]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[EventView]
AS
SELECT
    *
FROM
    [dbo].[Event]
GO
/****** Object:  View [dbo].[FolderView]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[FolderView]
AS
SELECT
    *
FROM
    [dbo].[Folder]
GO
/****** Object:  View [dbo].[GrantView]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[GrantView]
AS
SELECT
    *
FROM
    [dbo].[Grant]
GO
/****** Object:  View [dbo].[GroupView]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[GroupView]
AS
SELECT
    *
FROM
    [dbo].[Group]
GO
/****** Object:  View [dbo].[InstallationView]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[InstallationView]
AS
SELECT
    *
FROM
    [dbo].[Installation]
GO
/****** Object:  View [dbo].[OrganizationUserOrganizationDetailsView]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[OrganizationUserOrganizationDetailsView]
AS
SELECT
    OU.[UserId],
    OU.[OrganizationId],
    O.[Name],
    O.[Enabled],
    O.[UsePolicies],
    O.[UseSso],
    O.[UseGroups],
    O.[UseDirectory],
    O.[UseEvents],
    O.[UseTotp],
    O.[Use2fa],
    O.[UseApi],
    O.[SelfHost],
    O.[UsersGetPremium],
    O.[Seats],
    O.[MaxCollections],
    O.[MaxStorageGb],
    O.[Identifier],
    OU.[Key],
    OU.[Status],
    OU.[Type],
    SU.[ExternalId] SsoExternalId,
    OU.[Permissions]
FROM
    [dbo].[OrganizationUser] OU
INNER JOIN
    [dbo].[Organization] O ON O.[Id] = OU.[OrganizationId]
LEFT JOIN
    [dbo].[SsoUser] SU ON SU.[UserId] = OU.[UserId] AND SU.[OrganizationId] = OU.[OrganizationId]
GO
/****** Object:  View [dbo].[OrganizationUserUserDetailsView]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[OrganizationUserUserDetailsView]
AS
SELECT
    OU.[Id],
    OU.[UserId],
    OU.[OrganizationId],
    U.[Name],
    ISNULL(U.[Email], OU.[Email]) Email,
    U.[TwoFactorProviders],
    U.[Premium],
    OU.[Status],
    OU.[Type],
    OU.[AccessAll],
    OU.[ExternalId],
    SU.[ExternalId] SsoExternalId,
    OU.[Permissions]
FROM
    [dbo].[OrganizationUser] OU
LEFT JOIN
    [dbo].[User] U ON U.[Id] = OU.[UserId]
LEFT JOIN
    [dbo].[SsoUser] SU ON SU.[UserId] = OU.[UserId] AND SU.[OrganizationId] = OU.[OrganizationId]
GO
/****** Object:  View [dbo].[OrganizationUserView]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[OrganizationUserView]
AS
SELECT
    *
FROM
    [dbo].[OrganizationUser]
GO
/****** Object:  View [dbo].[OrganizationView]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[OrganizationView]
AS
SELECT
    *
FROM
    [dbo].[Organization]
GO
/****** Object:  View [dbo].[PolicyView]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[PolicyView]
AS
SELECT
    *
FROM
    [dbo].[Policy]
GO
/****** Object:  View [dbo].[SendView]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[SendView]
AS
SELECT
    *
FROM
    [dbo].[Send]
GO
/****** Object:  View [dbo].[SsoConfigView]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[SsoConfigView]
AS
SELECT
    *
FROM
    [dbo].[SsoConfig]
GO
/****** Object:  View [dbo].[TransactionView]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[TransactionView]
AS
SELECT
    *
FROM
    [dbo].[Transaction]
GO
/****** Object:  View [dbo].[U2fView]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[U2fView]
AS
SELECT
    *
FROM
    [dbo].[U2f]
GO
/****** Object:  View [dbo].[UserView]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[UserView]
AS
SELECT
    *
FROM
    [dbo].[User]
GO
/****** Object:  Index [IX_Cipher_OrganizationId]    Script Date: 16/06/2021 10:30:53 ******/
CREATE NONCLUSTERED INDEX [IX_Cipher_OrganizationId] ON [dbo].[Cipher]
(
	[OrganizationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Cipher_UserId_OrganizationId_IncludeAll]    Script Date: 16/06/2021 10:30:53 ******/
CREATE NONCLUSTERED INDEX [IX_Cipher_UserId_OrganizationId_IncludeAll] ON [dbo].[Cipher]
(
	[UserId] ASC,
	[OrganizationId] ASC
)
INCLUDE ( 	[Type],
	[Data],
	[Favorites],
	[Folders],
	[Attachments],
	[CreationDate],
	[RevisionDate],
	[DeletedDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Collection_OrganizationId_IncludeAll]    Script Date: 16/06/2021 10:30:53 ******/
CREATE NONCLUSTERED INDEX [IX_Collection_OrganizationId_IncludeAll] ON [dbo].[Collection]
(
	[OrganizationId] ASC
)
INCLUDE ( 	[CreationDate],
	[Name],
	[RevisionDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_CollectionCipher_CipherId]    Script Date: 16/06/2021 10:30:53 ******/
CREATE NONCLUSTERED INDEX [IX_CollectionCipher_CipherId] ON [dbo].[CollectionCipher]
(
	[CipherId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Device_Identifier]    Script Date: 16/06/2021 10:30:53 ******/
CREATE NONCLUSTERED INDEX [IX_Device_Identifier] ON [dbo].[Device]
(
	[Identifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [UX_Device_UserId_Identifier]    Script Date: 16/06/2021 10:30:53 ******/
CREATE UNIQUE NONCLUSTERED INDEX [UX_Device_UserId_Identifier] ON [dbo].[Device]
(
	[UserId] ASC,
	[Identifier] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Event_DateOrganizationIdUserId]    Script Date: 16/06/2021 10:30:53 ******/
CREATE NONCLUSTERED INDEX [IX_Event_DateOrganizationIdUserId] ON [dbo].[Event]
(
	[Date] DESC,
	[OrganizationId] ASC,
	[ActingUserId] ASC,
	[CipherId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Folder_UserId_IncludeAll]    Script Date: 16/06/2021 10:30:53 ******/
CREATE NONCLUSTERED INDEX [IX_Folder_UserId_IncludeAll] ON [dbo].[Folder]
(
	[UserId] ASC
)
INCLUDE ( 	[Name],
	[CreationDate],
	[RevisionDate]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Grant_ExpirationDate]    Script Date: 16/06/2021 10:30:53 ******/
CREATE NONCLUSTERED INDEX [IX_Grant_ExpirationDate] ON [dbo].[Grant]
(
	[ExpirationDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Grant_SubjectId_ClientId_Type]    Script Date: 16/06/2021 10:30:53 ******/
CREATE NONCLUSTERED INDEX [IX_Grant_SubjectId_ClientId_Type] ON [dbo].[Grant]
(
	[SubjectId] ASC,
	[ClientId] ASC,
	[Type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Grant_SubjectId_SessionId_Type]    Script Date: 16/06/2021 10:30:53 ******/
CREATE NONCLUSTERED INDEX [IX_Grant_SubjectId_SessionId_Type] ON [dbo].[Grant]
(
	[SubjectId] ASC,
	[SessionId] ASC,
	[Type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_GroupUser_OrganizationUserId]    Script Date: 16/06/2021 10:30:53 ******/
CREATE NONCLUSTERED INDEX [IX_GroupUser_OrganizationUserId] ON [dbo].[GroupUser]
(
	[OrganizationUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Organization_Enabled]    Script Date: 16/06/2021 10:30:53 ******/
CREATE NONCLUSTERED INDEX [IX_Organization_Enabled] ON [dbo].[Organization]
(
	[Id] ASC,
	[Enabled] ASC
)
INCLUDE ( 	[UseTotp]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Organization_Identifier]    Script Date: 16/06/2021 10:30:53 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Organization_Identifier] ON [dbo].[Organization]
(
	[Identifier] ASC
)
WHERE ([Identifier] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrganizationUser_OrganizationId]    Script Date: 16/06/2021 10:30:53 ******/
CREATE NONCLUSTERED INDEX [IX_OrganizationUser_OrganizationId] ON [dbo].[OrganizationUser]
(
	[OrganizationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_OrganizationUser_UserIdOrganizationIdStatus]    Script Date: 16/06/2021 10:30:53 ******/
CREATE NONCLUSTERED INDEX [IX_OrganizationUser_UserIdOrganizationIdStatus] ON [dbo].[OrganizationUser]
(
	[UserId] ASC,
	[OrganizationId] ASC,
	[Status] ASC
)
INCLUDE ( 	[AccessAll]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Policy_OrganizationId_Type]    Script Date: 16/06/2021 10:30:53 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Policy_OrganizationId_Type] ON [dbo].[Policy]
(
	[OrganizationId] ASC,
	[Type] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Send_DeletionDate]    Script Date: 16/06/2021 10:30:53 ******/
CREATE NONCLUSTERED INDEX [IX_Send_DeletionDate] ON [dbo].[Send]
(
	[DeletionDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Send_UserId_OrganizationId]    Script Date: 16/06/2021 10:30:53 ******/
CREATE NONCLUSTERED INDEX [IX_Send_UserId_OrganizationId] ON [dbo].[Send]
(
	[UserId] ASC,
	[OrganizationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_SsoUser_OrganizationIdExternalId]    Script Date: 16/06/2021 10:30:53 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_SsoUser_OrganizationIdExternalId] ON [dbo].[SsoUser]
(
	[OrganizationId] ASC,
	[ExternalId] ASC
)
INCLUDE ( 	[UserId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_SsoUser_OrganizationIdUserId]    Script Date: 16/06/2021 10:30:53 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_SsoUser_OrganizationIdUserId] ON [dbo].[SsoUser]
(
	[OrganizationId] ASC,
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_TaxRate_Country_PostalCode_Active_Uniqueness]    Script Date: 16/06/2021 10:30:53 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_TaxRate_Country_PostalCode_Active_Uniqueness] ON [dbo].[TaxRate]
(
	[Country] ASC,
	[PostalCode] ASC
)
WHERE ([Active]=(1))
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_Transaction_Gateway_GatewayId]    Script Date: 16/06/2021 10:30:53 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_Transaction_Gateway_GatewayId] ON [dbo].[Transaction]
(
	[Gateway] ASC,
	[GatewayId] ASC
)
WHERE ([Gateway] IS NOT NULL AND [GatewayId] IS NOT NULL)
WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_Transaction_UserId_OrganizationId_CreationDate]    Script Date: 16/06/2021 10:30:53 ******/
CREATE NONCLUSTERED INDEX [IX_Transaction_UserId_OrganizationId_CreationDate] ON [dbo].[Transaction]
(
	[UserId] ASC,
	[OrganizationId] ASC,
	[CreationDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_U2f_CreationDate]    Script Date: 16/06/2021 10:30:53 ******/
CREATE NONCLUSTERED INDEX [IX_U2f_CreationDate] ON [dbo].[U2f]
(
	[CreationDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_U2f_UserId]    Script Date: 16/06/2021 10:30:53 ******/
CREATE NONCLUSTERED INDEX [IX_U2f_UserId] ON [dbo].[U2f]
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
SET ANSI_PADDING ON

GO
/****** Object:  Index [IX_User_Email]    Script Date: 16/06/2021 10:30:53 ******/
CREATE UNIQUE NONCLUSTERED INDEX [IX_User_Email] ON [dbo].[User]
(
	[Email] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
/****** Object:  Index [IX_User_Premium_PremiumExpirationDate_RenewalReminderDate]    Script Date: 16/06/2021 10:30:53 ******/
CREATE NONCLUSTERED INDEX [IX_User_Premium_PremiumExpirationDate_RenewalReminderDate] ON [dbo].[User]
(
	[Premium] ASC,
	[PremiumExpirationDate] ASC,
	[RenewalReminderDate] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Cipher]  WITH NOCHECK ADD  CONSTRAINT [FK_Cipher_Organization] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organization] ([Id])
GO
ALTER TABLE [dbo].[Cipher] CHECK CONSTRAINT [FK_Cipher_Organization]
GO
ALTER TABLE [dbo].[Cipher]  WITH NOCHECK ADD  CONSTRAINT [FK_Cipher_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Cipher] CHECK CONSTRAINT [FK_Cipher_User]
GO
ALTER TABLE [dbo].[Collection]  WITH NOCHECK ADD  CONSTRAINT [FK_Collection_Organization] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organization] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Collection] CHECK CONSTRAINT [FK_Collection_Organization]
GO
ALTER TABLE [dbo].[CollectionCipher]  WITH CHECK ADD  CONSTRAINT [FK_CollectionCipher_Cipher] FOREIGN KEY([CipherId])
REFERENCES [dbo].[Cipher] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CollectionCipher] CHECK CONSTRAINT [FK_CollectionCipher_Cipher]
GO
ALTER TABLE [dbo].[CollectionCipher]  WITH CHECK ADD  CONSTRAINT [FK_CollectionCipher_Collection] FOREIGN KEY([CollectionId])
REFERENCES [dbo].[Collection] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CollectionCipher] CHECK CONSTRAINT [FK_CollectionCipher_Collection]
GO
ALTER TABLE [dbo].[CollectionGroup]  WITH CHECK ADD  CONSTRAINT [FK_CollectionGroup_Collection] FOREIGN KEY([CollectionId])
REFERENCES [dbo].[Collection] ([Id])
GO
ALTER TABLE [dbo].[CollectionGroup] CHECK CONSTRAINT [FK_CollectionGroup_Collection]
GO
ALTER TABLE [dbo].[CollectionGroup]  WITH CHECK ADD  CONSTRAINT [FK_CollectionGroup_Group] FOREIGN KEY([GroupId])
REFERENCES [dbo].[Group] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CollectionGroup] CHECK CONSTRAINT [FK_CollectionGroup_Group]
GO
ALTER TABLE [dbo].[CollectionUser]  WITH CHECK ADD  CONSTRAINT [FK_CollectionUser_Collection] FOREIGN KEY([CollectionId])
REFERENCES [dbo].[Collection] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[CollectionUser] CHECK CONSTRAINT [FK_CollectionUser_Collection]
GO
ALTER TABLE [dbo].[CollectionUser]  WITH CHECK ADD  CONSTRAINT [FK_CollectionUser_OrganizationUser] FOREIGN KEY([OrganizationUserId])
REFERENCES [dbo].[OrganizationUser] ([Id])
GO
ALTER TABLE [dbo].[CollectionUser] CHECK CONSTRAINT [FK_CollectionUser_OrganizationUser]
GO
ALTER TABLE [dbo].[Device]  WITH CHECK ADD  CONSTRAINT [FK_Device_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Device] CHECK CONSTRAINT [FK_Device_User]
GO
ALTER TABLE [dbo].[EmergencyAccess]  WITH NOCHECK ADD  CONSTRAINT [FK_EmergencyAccess_GranteeId] FOREIGN KEY([GranteeId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[EmergencyAccess] CHECK CONSTRAINT [FK_EmergencyAccess_GranteeId]
GO
ALTER TABLE [dbo].[EmergencyAccess]  WITH NOCHECK ADD  CONSTRAINT [FK_EmergencyAccess_GrantorId] FOREIGN KEY([GrantorId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[EmergencyAccess] CHECK CONSTRAINT [FK_EmergencyAccess_GrantorId]
GO
ALTER TABLE [dbo].[Folder]  WITH NOCHECK ADD  CONSTRAINT [FK_Folder_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Folder] CHECK CONSTRAINT [FK_Folder_User]
GO
ALTER TABLE [dbo].[Group]  WITH CHECK ADD  CONSTRAINT [FK_Group_Organization] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organization] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Group] CHECK CONSTRAINT [FK_Group_Organization]
GO
ALTER TABLE [dbo].[GroupUser]  WITH CHECK ADD  CONSTRAINT [FK_GroupUser_Group] FOREIGN KEY([GroupId])
REFERENCES [dbo].[Group] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[GroupUser] CHECK CONSTRAINT [FK_GroupUser_Group]
GO
ALTER TABLE [dbo].[GroupUser]  WITH CHECK ADD  CONSTRAINT [FK_GroupUser_OrganizationUser] FOREIGN KEY([OrganizationUserId])
REFERENCES [dbo].[OrganizationUser] ([Id])
GO
ALTER TABLE [dbo].[GroupUser] CHECK CONSTRAINT [FK_GroupUser_OrganizationUser]
GO
ALTER TABLE [dbo].[OrganizationUser]  WITH CHECK ADD  CONSTRAINT [FK_OrganizationUser_Organization] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organization] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[OrganizationUser] CHECK CONSTRAINT [FK_OrganizationUser_Organization]
GO
ALTER TABLE [dbo].[OrganizationUser]  WITH CHECK ADD  CONSTRAINT [FK_OrganizationUser_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[OrganizationUser] CHECK CONSTRAINT [FK_OrganizationUser_User]
GO
ALTER TABLE [dbo].[Policy]  WITH CHECK ADD  CONSTRAINT [FK_Policy_Organization] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organization] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Policy] CHECK CONSTRAINT [FK_Policy_Organization]
GO
ALTER TABLE [dbo].[Send]  WITH CHECK ADD  CONSTRAINT [FK_Send_Organization] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organization] ([Id])
GO
ALTER TABLE [dbo].[Send] CHECK CONSTRAINT [FK_Send_Organization]
GO
ALTER TABLE [dbo].[Send]  WITH CHECK ADD  CONSTRAINT [FK_Send_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[Send] CHECK CONSTRAINT [FK_Send_User]
GO
ALTER TABLE [dbo].[SsoConfig]  WITH CHECK ADD  CONSTRAINT [FK_SsoConfig_Organization] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organization] ([Id])
GO
ALTER TABLE [dbo].[SsoConfig] CHECK CONSTRAINT [FK_SsoConfig_Organization]
GO
ALTER TABLE [dbo].[SsoUser]  WITH CHECK ADD  CONSTRAINT [FK_SsoUser_Organization] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organization] ([Id])
GO
ALTER TABLE [dbo].[SsoUser] CHECK CONSTRAINT [FK_SsoUser_Organization]
GO
ALTER TABLE [dbo].[SsoUser]  WITH CHECK ADD  CONSTRAINT [FK_SsoUser_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[SsoUser] CHECK CONSTRAINT [FK_SsoUser_User]
GO
ALTER TABLE [dbo].[Transaction]  WITH CHECK ADD  CONSTRAINT [FK_Transaction_Organization] FOREIGN KEY([OrganizationId])
REFERENCES [dbo].[Organization] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Transaction] CHECK CONSTRAINT [FK_Transaction_Organization]
GO
ALTER TABLE [dbo].[Transaction]  WITH CHECK ADD  CONSTRAINT [FK_Transaction_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Transaction] CHECK CONSTRAINT [FK_Transaction_User]
GO
ALTER TABLE [dbo].[U2f]  WITH CHECK ADD  CONSTRAINT [FK_U2f_User] FOREIGN KEY([UserId])
REFERENCES [dbo].[User] ([Id])
GO
ALTER TABLE [dbo].[U2f] CHECK CONSTRAINT [FK_U2f_User]
GO
/****** Object:  StoredProcedure [dbo].[AzureSQLMaintenance]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE Procedure [dbo].[AzureSQLMaintenance]
    (
        @operation nvarchar(10) = null,
        @mode nvarchar(10) = 'smart',
        @LogToTable bit = 0
    )
as
begin
    set nocount on
    declare @msg nvarchar(max);
    declare @minPageCountForIndex int = 40;
    declare @OperationTime datetime2 = sysdatetime();
    declare @KeepXOperationInLog int =3;

    /* make sure parameters selected correctly */
    set @operation = lower(@operation)
    set @mode = lower(@mode)
    
    if @mode not in ('smart','dummy')
        set @mode = 'smart'

    if @operation not in ('index','statistics','all') or @operation is null
    begin
        raiserror('@operation (varchar(10)) [mandatory]',0,0)
        raiserror(' Select operation to perform:',0,0)
        raiserror('     "index" to perform index maintenance',0,0)
        raiserror('     "statistics" to perform statistics maintenance',0,0)
        raiserror('     "all" to perform indexes and statistics maintenance',0,0)
        raiserror(' ',0,0)
        raiserror('@mode(varchar(10)) [optional]',0,0)
        raiserror(' optionaly you can supply second parameter for operation mode: ',0,0)
        raiserror('     "smart" (Default) using smart decition about what index or stats should be touched.',0,0)
        raiserror('     "dummy" going through all indexes and statistics regardless thier modifications or fragmentation.',0,0)
        raiserror(' ',0,0)
        raiserror('@LogToTable(bit) [optional]',0,0)
        raiserror(' Logging option: @LogToTable(bit)',0,0)
        raiserror('     0 - (Default) do not log operation to table',0,0)
        raiserror('     1 - log operation to table',0,0)
        raiserror('		for logging option only 3 last execution will be kept by default. this can be changed by easily in the procedure body.',0,0)
        raiserror('		Log table will be created automatically if not exists.',0,0)
    end
    else 
    begin
        /*Write operation parameters*/
        raiserror('-----------------------',0,0)
        set @msg = 'set operation = ' + @operation;
        raiserror(@msg,0,0)
        set @msg = 'set mode = ' + @mode;
        raiserror(@msg,0,0)
        set @msg = 'set LogToTable = ' + cast(@LogToTable as varchar(1));
        raiserror(@msg,0,0)
        raiserror('-----------------------',0,0)
    end
    
    /* Prepare Log Table */
        if object_id('AzureSQLMaintenanceLog') is null 
        begin
            create table AzureSQLMaintenanceLog (id bigint primary key identity(1,1), OperationTime datetime2, command varchar(4000),ExtraInfo varchar(4000), StartTime datetime2, EndTime datetime2, StatusMessage varchar(1000));
        end

    if @LogToTable=1 insert into AzureSQLMaintenanceLog values(@OperationTime,null,null,sysdatetime(),sysdatetime(),'Starting operation: Operation=' +@operation + ' Mode=' + @mode + ' Keep log for last ' + cast(@KeepXOperationInLog as varchar(10)) + ' operations' )	

    create table #cmdQueue (txtCMD nvarchar(max),ExtraInfo varchar(max))


    if @operation in('index','all')
    begin
        raiserror('Get index information...(wait)',0,0) with nowait;
        /* Get Index Information */
        select 
            i.[object_id]
            ,ObjectSchema = OBJECT_SCHEMA_NAME(i.object_id)
            ,ObjectName = object_name(i.object_id) 
            ,IndexName = idxs.name
            ,i.avg_fragmentation_in_percent
            ,i.page_count
            ,i.index_id
            ,i.partition_number
            ,i.index_type_desc
            ,i.avg_page_space_used_in_percent
            ,i.record_count
            ,i.ghost_record_count
            ,i.forwarded_record_count
            ,null as OnlineOpIsNotSupported
        into #idxBefore
        from sys.dm_db_index_physical_stats(DB_ID(),NULL, NULL, NULL ,'limited') i
        left join sys.indexes idxs on i.object_id = idxs.object_id and i.index_id = idxs.index_id
        where idxs.type in (1/*Clustered index*/,2/*NonClustered index*/) /*Avoid HEAPS*/
        order by i.avg_fragmentation_in_percent desc, page_count desc


        -- mark indexes XML,spatial and columnstore not to run online update 
        update #idxBefore set OnlineOpIsNotSupported=1 where [object_id] in (select [object_id] from #idxBefore where index_id >=1000)
        
        
        raiserror('---------------------------------------',0,0) with nowait
        raiserror('Index Information:',0,0) with nowait
        raiserror('---------------------------------------',0,0) with nowait

        select @msg = count(*) from #idxBefore where index_id in (1,2)
        set @msg = 'Total Indexes: ' + @msg
        raiserror(@msg,0,0) with nowait

        select @msg = avg(avg_fragmentation_in_percent) from #idxBefore where index_id in (1,2) and page_count>@minPageCountForIndex
        set @msg = 'Average Fragmentation: ' + @msg
        raiserror(@msg,0,0) with nowait

        select @msg = sum(iif(avg_fragmentation_in_percent>=5 and page_count>@minPageCountForIndex,1,0)) from #idxBefore where index_id in (1,2)
        set @msg = 'Fragmented Indexes: ' + @msg
        raiserror(@msg,0,0) with nowait

                
        raiserror('---------------------------------------',0,0) with nowait

            
            
            
        /* create queue for update indexes */
        insert into #cmdQueue
        select 
        txtCMD = 
        case when avg_fragmentation_in_percent>5 and avg_fragmentation_in_percent<30 and @mode = 'smart' then
            'ALTER INDEX [' + IndexName + '] ON [' + ObjectSchema + '].[' + ObjectName + '] REORGANIZE;'
            when OnlineOpIsNotSupported=1 then
            'ALTER INDEX [' + IndexName + '] ON [' + ObjectSchema + '].[' + ObjectName + '] REBUILD WITH(ONLINE=OFF,MAXDOP=1);'
            else
            'ALTER INDEX [' + IndexName + '] ON [' + ObjectSchema + '].[' + ObjectName + '] REBUILD WITH(ONLINE=ON,MAXDOP=1);'
        end
        , ExtraInfo = 'Current fragmentation: ' + format(avg_fragmentation_in_percent/100,'p')
        from #idxBefore
        where 
            index_id>0 /*disable heaps*/ 
            and index_id < 1000 /* disable XML indexes */
            --
            and 
                (
                    page_count> @minPageCountForIndex and /* not small tables */
                    avg_fragmentation_in_percent>=5
                )
            or
                (
                    @mode ='dummy'
                )
    end

    if @operation in('statistics','all')
    begin 
        /*Gets Stats for database*/
        raiserror('Get statistics information...',0,0) with nowait;
        select 
            ObjectSchema = OBJECT_SCHEMA_NAME(s.object_id)
            ,ObjectName = object_name(s.object_id) 
            ,StatsName = s.name
            ,sp.last_updated
            ,sp.rows
            ,sp.rows_sampled
            ,sp.modification_counter
        into #statsBefore
        from sys.stats s cross apply sys.dm_db_stats_properties(s.object_id,s.stats_id) sp 
        where OBJECT_SCHEMA_NAME(s.object_id) != 'sys' and (sp.modification_counter>0 or @mode='dummy')
        order by sp.last_updated asc

        
        raiserror('---------------------------------------',0,0) with nowait
        raiserror('Statistics Information:',0,0) with nowait
        raiserror('---------------------------------------',0,0) with nowait

        select @msg = sum(modification_counter) from #statsBefore
        set @msg = 'Total Modifications: ' + @msg
        raiserror(@msg,0,0) with nowait
        
        select @msg = sum(iif(modification_counter>0,1,0)) from #statsBefore
        set @msg = 'Modified Statistics: ' + @msg
        raiserror(@msg,0,0) with nowait
                
        raiserror('---------------------------------------',0,0) with nowait




        /* create queue for update stats */
        insert into #cmdQueue
        select 
        txtCMD = 'UPDATE STATISTICS [' + ObjectSchema + '].[' + ObjectName + '] (['+ StatsName +']) WITH FULLSCAN;'
        , ExtraInfo = '#rows:' + cast([rows] as varchar(100)) + ' #modifications:' + cast(modification_counter as varchar(100)) + ' modification percent: ' + format((1.0 * modification_counter/ rows ),'p')
        from #statsBefore
    end


    if @operation in('statistics','index','all')
    begin 
        /* iterate through all stats */
        raiserror('Start executing commands...',0,0) with nowait
        declare @SQLCMD nvarchar(max);
        declare @ExtraInfo nvarchar(max);
        declare @T table(txtCMD nvarchar(max),ExtraInfo nvarchar(max));
        while exists(select * from #cmdQueue)
        begin
            delete top (1) from #cmdQueue output deleted.* into @T;
            select top (1) @SQLCMD = txtCMD, @ExtraInfo=ExtraInfo from @T
            raiserror(@SQLCMD,0,0) with nowait
            if @LogToTable=1 insert into AzureSQLMaintenanceLog values(@OperationTime,@SQLCMD,@ExtraInfo,sysdatetime(),null,'Started')
            begin try
                exec(@SQLCMD)	
                if @LogToTable=1 update AzureSQLMaintenanceLog set EndTime = sysdatetime(), StatusMessage = 'Succeeded' where id=SCOPE_IDENTITY()
            end try
            begin catch
                raiserror('cached',0,0) with nowait
                if @LogToTable=1 update AzureSQLMaintenanceLog set EndTime = sysdatetime(), StatusMessage = 'FAILED : ' + CAST(ERROR_NUMBER() AS VARCHAR(50)) + ERROR_MESSAGE() where id=SCOPE_IDENTITY()
            end catch
            delete from @T
        end
    end
    
    /* Clean old records from log table */
    if @LogToTable=1
    begin
        delete from AzureSQLMaintenanceLog 
        from 
            AzureSQLMaintenanceLog L join 
            (select distinct OperationTime from AzureSQLMaintenanceLog order by OperationTime desc offset @KeepXOperationInLog rows) F
                ON L.OperationTime = F.OperationTime
        insert into AzureSQLMaintenanceLog values(@OperationTime,null,cast(@@rowcount as varchar(100))+ ' rows purged from log table because number of operations to keep is set to: ' + cast( @KeepXOperationInLog as varchar(100)),sysdatetime(),sysdatetime(),'Cleanup Log Table')
    end

    raiserror('Done',0,0)
    if @LogToTable=1 insert into AzureSQLMaintenanceLog values(@OperationTime,null,null,sysdatetime(),sysdatetime(),'End of operation')
end
GO
/****** Object:  StoredProcedure [dbo].[Cipher_Create]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Cipher_Create]
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @Type TINYINT,
    @Data NVARCHAR(MAX),
    @Favorites NVARCHAR(MAX),
    @Folders NVARCHAR(MAX),
    @Attachments NVARCHAR(MAX),
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7),
    @DeletedDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [dbo].[Cipher]
    (
        [Id],
        [UserId],
        [OrganizationId],
        [Type],
        [Data],
        [Favorites],
        [Folders],
        [Attachments],
        [CreationDate],
        [RevisionDate],
        [DeletedDate]
    )
    VALUES
    (
        @Id,
        CASE WHEN @OrganizationId IS NULL THEN @UserId ELSE NULL END,
        @OrganizationId,
        @Type,
        @Data,
        @Favorites,
        @Folders,
        @Attachments,
        @CreationDate,
        @RevisionDate,
        @DeletedDate
    )

    IF @OrganizationId IS NOT NULL
    BEGIN
        EXEC [dbo].[User_BumpAccountRevisionDateByCipherId] @Id, @OrganizationId
    END
    ELSE IF @UserId IS NOT NULL
    BEGIN
        EXEC [dbo].[User_BumpAccountRevisionDate] @UserId
    END
END
GO
/****** Object:  StoredProcedure [dbo].[Cipher_CreateWithCollections]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Cipher_CreateWithCollections]
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @Type TINYINT,
    @Data NVARCHAR(MAX),
    @Favorites NVARCHAR(MAX),
    @Folders NVARCHAR(MAX),
    @Attachments NVARCHAR(MAX),
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7),
    @DeletedDate DATETIME2(7),
    @CollectionIds AS [dbo].[GuidIdArray] READONLY
AS
BEGIN
    SET NOCOUNT ON

    EXEC [dbo].[Cipher_Create] @Id, @UserId, @OrganizationId, @Type, @Data, @Favorites, @Folders,
        @Attachments, @CreationDate, @RevisionDate, @DeletedDate

    DECLARE @UpdateCollectionsSuccess INT
    EXEC @UpdateCollectionsSuccess = [dbo].[Cipher_UpdateCollections] @Id, @UserId, @OrganizationId, @CollectionIds
END
GO
/****** Object:  StoredProcedure [dbo].[Cipher_Delete]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Cipher_Delete]
    @Ids AS [dbo].[GuidIdArray] READONLY,
    @UserId AS UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    CREATE TABLE #Temp
    ( 
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [UserId] UNIQUEIDENTIFIER NULL,
        [OrganizationId] UNIQUEIDENTIFIER NULL,
        [Attachments] BIT NOT NULL
    )

    INSERT INTO #Temp
    SELECT
        [Id],
        [UserId],
        [OrganizationId],
        CASE WHEN [Attachments] IS NULL THEN 0 ELSE 1 END
    FROM
        [dbo].[UserCipherDetails](@UserId)
    WHERE
        [Edit] = 1
        AND [Id] IN (SELECT * FROM @Ids)

    -- Delete ciphers
    DELETE
    FROM
        [dbo].[Cipher]
    WHERE
        [Id] IN (SELECT [Id] FROM #Temp)

    -- Cleanup orgs
    DECLARE @OrgId UNIQUEIDENTIFIER
    DECLARE [OrgCursor] CURSOR FORWARD_ONLY FOR
        SELECT
            [OrganizationId]
        FROM
            #Temp
        WHERE
            [OrganizationId] IS NOT NULL
        GROUP BY
            [OrganizationId]
    OPEN [OrgCursor]
    FETCH NEXT FROM [OrgCursor] INTO @OrgId
    WHILE @@FETCH_STATUS = 0 BEGIN
        EXEC [dbo].[Organization_UpdateStorage] @OrgId
        EXEC [dbo].[User_BumpAccountRevisionDateByOrganizationId] @OrgId
        FETCH NEXT FROM [OrgCursor] INTO @OrgId
    END
    CLOSE [OrgCursor]
    DEALLOCATE [OrgCursor]

    -- Cleanup user
    DECLARE @UserCiphersWithStorageCount INT
    SELECT
        @UserCiphersWithStorageCount = COUNT(1)
    FROM
        #Temp
    WHERE
        [UserId] IS NOT NULL
        AND [Attachments] = 1

    IF @UserCiphersWithStorageCount > 0
    BEGIN
        EXEC [dbo].[User_UpdateStorage] @UserId
    END
    EXEC [dbo].[User_BumpAccountRevisionDate] @UserId

    DROP TABLE #Temp
END
GO
/****** Object:  StoredProcedure [dbo].[Cipher_DeleteAttachment]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Cipher_DeleteAttachment]
    @Id UNIQUEIDENTIFIER,
    @AttachmentId VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @AttachmentIdKey VARCHAR(50) = CONCAT('"', @AttachmentId, '"')
    DECLARE @AttachmentIdPath VARCHAR(50) = CONCAT('$.', @AttachmentIdKey)

    DECLARE @UserId UNIQUEIDENTIFIER
    DECLARE @OrganizationId UNIQUEIDENTIFIER

    SELECT
        @UserId = [UserId],
        @OrganizationId = [OrganizationId]
    FROM 
        [dbo].[Cipher]
    WHERE [Id] = @Id

    UPDATE
        [dbo].[Cipher]
    SET
        [Attachments] = JSON_MODIFY([Attachments], @AttachmentIdPath, NULL)
    WHERE
        [Id] = @Id

    IF @OrganizationId IS NOT NULL
    BEGIN
        EXEC [dbo].[Organization_UpdateStorage] @OrganizationId
        EXEC [dbo].[User_BumpAccountRevisionDateByCipherId] @Id, @OrganizationId
    END
    ELSE IF @UserId IS NOT NULL
    BEGIN
        EXEC [dbo].[User_UpdateStorage] @UserId
        EXEC [dbo].[User_BumpAccountRevisionDate] @UserId
    END
END
GO
/****** Object:  StoredProcedure [dbo].[Cipher_DeleteById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Cipher_DeleteById]
    @Id UNIQUEIDENTIFIER
WITH RECOMPILE
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @UserId UNIQUEIDENTIFIER
    DECLARE @OrganizationId UNIQUEIDENTIFIER
    DECLARE @Attachments BIT

    SELECT TOP 1
        @UserId = [UserId],
        @OrganizationId = [OrganizationId],
        @Attachments = CASE WHEN [Attachments] IS NOT NULL THEN 1 ELSE 0 END
    FROM
        [dbo].[Cipher]
    WHERE
        [Id] = @Id

    DELETE
    FROM
        [dbo].[Cipher]
    WHERE
        [Id] = @Id

    IF @OrganizationId IS NOT NULL
    BEGIN
        IF @Attachments = 1
        BEGIN
            EXEC [dbo].[Organization_UpdateStorage] @OrganizationId
        END
        EXEC [dbo].[User_BumpAccountRevisionDateByCipherId] @Id, @OrganizationId
    END
    ELSE IF @UserId IS NOT NULL
    BEGIN
        IF @Attachments = 1
        BEGIN
            EXEC [dbo].[User_UpdateStorage] @UserId
        END
        EXEC [dbo].[User_BumpAccountRevisionDate] @UserId
    END
END
GO
/****** Object:  StoredProcedure [dbo].[Cipher_DeleteByIdsOrganizationId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Cipher_DeleteByIdsOrganizationId]
    @Ids AS [dbo].[GuidIdArray] READONLY,
    @OrganizationId AS UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    -- Delete ciphers
    DELETE
    FROM
        [dbo].[Cipher]
    WHERE
        [Id] IN (SELECT * FROM @Ids)
        AND OrganizationId = @OrganizationId

    -- Cleanup organization
    EXEC [dbo].[Organization_UpdateStorage] @OrganizationId
    EXEC [dbo].[User_BumpAccountRevisionDateByOrganizationId] @OrganizationId
END
GO
/****** Object:  StoredProcedure [dbo].[Cipher_DeleteByOrganizationId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Cipher_DeleteByOrganizationId]
    @OrganizationId AS UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @BatchSize INT = 100

    -- Delete collection ciphers
    WHILE @BatchSize > 0
    BEGIN
        BEGIN TRANSACTION Cipher_DeleteByOrganizationId_CC

        DELETE TOP(@BatchSize) CC
        FROM
            [dbo].[CollectionCipher] CC
        INNER JOIN
            [dbo].[Collection] C ON C.[Id] = CC.[CollectionId]
        WHERE
            C.[OrganizationId] = @OrganizationId

        SET @BatchSize = @@ROWCOUNT

        COMMIT TRANSACTION Cipher_DeleteByOrganizationId_CC
    END

    -- Reset batch size
    SET @BatchSize = 100

    -- Delete ciphers
    WHILE @BatchSize > 0
    BEGIN
        BEGIN TRANSACTION Cipher_DeleteByOrganizationId

        DELETE TOP(@BatchSize)
        FROM
            [dbo].[Cipher]
        WHERE
            [OrganizationId] = @OrganizationId

        SET @BatchSize = @@ROWCOUNT

        COMMIT TRANSACTION Cipher_DeleteByOrganizationId
    END

    -- Cleanup organization
    EXEC [dbo].[Organization_UpdateStorage] @OrganizationId
    EXEC [dbo].[User_BumpAccountRevisionDateByOrganizationId] @OrganizationId
END
GO
/****** Object:  StoredProcedure [dbo].[Cipher_DeleteByUserId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Cipher_DeleteByUserId]
    @UserId AS UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @BatchSize INT = 100

    -- Delete ciphers
    WHILE @BatchSize > 0
    BEGIN
        BEGIN TRANSACTION Cipher_DeleteByUserId_Ciphers

        DELETE TOP(@BatchSize)
        FROM
            [dbo].[Cipher]
        WHERE
            [UserId] = @UserId

        SET @BatchSize = @@ROWCOUNT

        COMMIT TRANSACTION Cipher_DeleteByUserId_Ciphers
    END

    -- Delete folders
    DELETE
    FROM
        [dbo].[Folder]
    WHERE
        [UserId] = @UserId

    -- Cleanup user
    EXEC [dbo].[User_UpdateStorage] @UserId
    EXEC [dbo].[User_BumpAccountRevisionDate] @UserId
END
GO
/****** Object:  StoredProcedure [dbo].[Cipher_Move]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Cipher_Move]
    @Ids AS [dbo].[GuidIdArray] READONLY,
    @FolderId AS UNIQUEIDENTIFIER,
    @UserId AS UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @UserIdKey VARCHAR(50) = CONCAT('"', @UserId, '"')
    DECLARE @UserIdPath VARCHAR(50) = CONCAT('$.', @UserIdKey)

    ;WITH [IdsToMoveCTE] AS (
        SELECT
            [Id]
        FROM
            [dbo].[UserCipherDetails](@UserId)
        WHERE
            [Edit] = 1
            AND [Id] IN (SELECT * FROM @Ids)
    )
    UPDATE
        [dbo].[Cipher]
    SET
        [Folders] = 
            CASE
            WHEN @FolderId IS NOT NULL AND [Folders] IS NULL THEN
                CONCAT('{', @UserIdKey, ':"', @FolderId, '"', '}')
            WHEN @FolderId IS NOT NULL THEN
                JSON_MODIFY([Folders], @UserIdPath, CAST(@FolderId AS VARCHAR(50)))
            ELSE
                JSON_MODIFY([Folders], @UserIdPath, NULL)
            END
    WHERE
        [Id] IN (SELECT * FROM [IdsToMoveCTE])

    EXEC [dbo].[User_BumpAccountRevisionDate] @UserId
END
GO
/****** Object:  StoredProcedure [dbo].[Cipher_ReadById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Cipher_ReadById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[CipherView]
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[Cipher_ReadByOrganizationId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Cipher_ReadByOrganizationId]
    @OrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[CipherView]
    WHERE
        [UserId] IS NULL
        AND [OrganizationId] = @OrganizationId
END
GO
/****** Object:  StoredProcedure [dbo].[Cipher_ReadCanEditByIdUserId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Cipher_ReadCanEditByIdUserId]
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @CanEdit BIT

    ;WITH [CTE] AS (
        SELECT
            CASE 
                WHEN C.[UserId] IS NOT NULL OR OU.[AccessAll] = 1 OR CU.[ReadOnly] = 0 OR G.[AccessAll] = 1 OR CG.[ReadOnly] = 0 THEN 1
                ELSE 0
            END [Edit]
        FROM
            [dbo].[Cipher] C
        LEFT JOIN
            [dbo].[Organization] O ON C.[UserId] IS NULL AND O.[Id] = C.[OrganizationId]
        LEFT JOIN
            [dbo].[OrganizationUser] OU ON OU.[OrganizationId] = O.[Id] AND OU.[UserId] = @UserId
        LEFT JOIN
            [dbo].[CollectionCipher] CC ON C.[UserId] IS NULL AND OU.[AccessAll] = 0 AND CC.[CipherId] = C.[Id]
        LEFT JOIN
            [dbo].[CollectionUser] CU ON CU.[CollectionId] = CC.[CollectionId] AND CU.[OrganizationUserId] = OU.[Id]
        LEFT JOIN
            [dbo].[GroupUser] GU ON C.[UserId] IS NULL AND CU.[CollectionId] IS NULL AND OU.[AccessAll] = 0 AND GU.[OrganizationUserId] = OU.[Id]
        LEFT JOIN
            [dbo].[Group] G ON G.[Id] = GU.[GroupId]
        LEFT JOIN
            [dbo].[CollectionGroup] CG ON G.[AccessAll] = 0 AND CG.[CollectionId] = CC.[CollectionId] AND CG.[GroupId] = GU.[GroupId]
        WHERE
            C.Id = @Id
            AND (
                C.[UserId] = @UserId
                OR (
                    C.[UserId] IS NULL
                    AND OU.[Status] = 2 -- 2 = Confirmed
                    AND O.[Enabled] = 1
                    AND (
                        OU.[AccessAll] = 1
                        OR CU.[CollectionId] IS NOT NULL
                        OR G.[AccessAll] = 1
                        OR CG.[CollectionId] IS NOT NULL
                    )
                )
            )
    )
    SELECT
        @CanEdit = CASE
            WHEN COUNT(1) > 0 THEN 1
            ELSE 0
        END
    FROM
        [CTE]
    WHERE
        [Edit] = 1

    SELECT @CanEdit
END
GO
/****** Object:  StoredProcedure [dbo].[Cipher_Restore]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Cipher_Restore]
    @Ids AS [dbo].[GuidIdArray] READONLY,
    @UserId AS UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    CREATE TABLE #Temp
    (
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [UserId] UNIQUEIDENTIFIER NULL,
        [OrganizationId] UNIQUEIDENTIFIER NULL
    )

    INSERT INTO #Temp
    SELECT
        [Id],
        [UserId],
        [OrganizationId]
    FROM
        [dbo].[UserCipherDetails](@UserId)
    WHERE
        [Edit] = 1
        AND [DeletedDate] IS NOT NULL
        AND [Id] IN (SELECT *
        FROM @Ids)

    DECLARE @UtcNow DATETIME2(7) = GETUTCDATE();
    UPDATE
        [dbo].[Cipher]
    SET
        [DeletedDate] = NULL,
        [RevisionDate] = @UtcNow
    WHERE
        [Id] IN (SELECT [Id]
    FROM #Temp)

    -- Bump orgs
    DECLARE @OrgId UNIQUEIDENTIFIER
    DECLARE [OrgCursor] CURSOR FORWARD_ONLY FOR
        SELECT
        [OrganizationId]
    FROM
        #Temp
    WHERE
            [OrganizationId] IS NOT NULL
    GROUP BY
            [OrganizationId]
    OPEN [OrgCursor]
    FETCH NEXT FROM [OrgCursor] INTO @OrgId
    WHILE @@FETCH_STATUS = 0 BEGIN
        EXEC [dbo].[User_BumpAccountRevisionDateByOrganizationId] @OrgId
        FETCH NEXT FROM [OrgCursor] INTO @OrgId
    END
    CLOSE [OrgCursor]
    DEALLOCATE [OrgCursor]

    -- Bump user
    EXEC [dbo].[User_BumpAccountRevisionDate] @UserId

    DROP TABLE #Temp
    
    SELECT @UtcNow
END
GO
/****** Object:  StoredProcedure [dbo].[Cipher_SoftDelete]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Cipher_SoftDelete]
    @Ids AS [dbo].[GuidIdArray] READONLY,
    @UserId AS UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    CREATE TABLE #Temp
    ( 
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [UserId] UNIQUEIDENTIFIER NULL,
        [OrganizationId] UNIQUEIDENTIFIER NULL
    )

    INSERT INTO #Temp
    SELECT
        [Id],
        [UserId],
        [OrganizationId]
    FROM
        [dbo].[UserCipherDetails](@UserId)
    WHERE
        [Edit] = 1
        AND [DeletedDate] IS NULL
        AND [Id] IN (SELECT * FROM @Ids)

    -- Delete ciphers
    DECLARE @UtcNow DATETIME2(7) = GETUTCDATE();
    UPDATE
        [dbo].[Cipher]
    SET
        [DeletedDate] = @UtcNow,
        [RevisionDate] = @UtcNow
    WHERE
        [Id] IN (SELECT [Id] FROM #Temp)

    -- Cleanup orgs
    DECLARE @OrgId UNIQUEIDENTIFIER
    DECLARE [OrgCursor] CURSOR FORWARD_ONLY FOR
        SELECT
            [OrganizationId]
        FROM
            #Temp
        WHERE
            [OrganizationId] IS NOT NULL
        GROUP BY
            [OrganizationId]
    OPEN [OrgCursor]
    FETCH NEXT FROM [OrgCursor] INTO @OrgId
    WHILE @@FETCH_STATUS = 0 BEGIN
        EXEC [dbo].[User_BumpAccountRevisionDateByOrganizationId] @OrgId
        FETCH NEXT FROM [OrgCursor] INTO @OrgId
    END
    CLOSE [OrgCursor]
    DEALLOCATE [OrgCursor]

    EXEC [dbo].[User_BumpAccountRevisionDate] @UserId

    DROP TABLE #Temp
END
GO
/****** Object:  StoredProcedure [dbo].[Cipher_SoftDeleteByIdsOrganizationId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Cipher_SoftDeleteByIdsOrganizationId]
    @Ids AS [dbo].[GuidIdArray] READONLY,
    @OrganizationId AS UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    -- Delete ciphers
    DECLARE @UtcNow DATETIME2(7) = GETUTCDATE();
    UPDATE
        [dbo].[Cipher]
    SET
        [DeletedDate] = @UtcNow,
        [RevisionDate] = @UtcNow
    WHERE
        [Id] IN (SELECT * FROM @Ids)
        AND OrganizationId = @OrganizationId

    -- Cleanup organization
    EXEC [dbo].[Organization_UpdateStorage] @OrganizationId
    EXEC [dbo].[User_BumpAccountRevisionDateByOrganizationId] @OrganizationId
END
GO
/****** Object:  StoredProcedure [dbo].[Cipher_Update]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Cipher_Update]
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @Type TINYINT,
    @Data NVARCHAR(MAX),
    @Favorites NVARCHAR(MAX),
    @Folders NVARCHAR(MAX),
    @Attachments NVARCHAR(MAX),
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7),
    @DeletedDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    UPDATE
        [dbo].[Cipher]
    SET
        [UserId] = CASE WHEN @OrganizationId IS NULL THEN @UserId ELSE NULL END,
        [OrganizationId] = @OrganizationId,
        [Type] = @Type,
        [Data] = @Data,
        [Favorites] = @Favorites,
        [Folders] = @Folders,
        [Attachments] = @Attachments,
        [CreationDate] = @CreationDate,
        [RevisionDate] = @RevisionDate,
        [DeletedDate] = @DeletedDate
    WHERE
        [Id] = @Id

    IF @OrganizationId IS NOT NULL
    BEGIN
        EXEC [dbo].[User_BumpAccountRevisionDateByCipherId] @Id, @OrganizationId
    END
    ELSE IF @UserId IS NOT NULL
    BEGIN
        EXEC [dbo].[User_BumpAccountRevisionDate] @UserId
    END
END
GO
/****** Object:  StoredProcedure [dbo].[Cipher_UpdateAttachment]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Cipher_UpdateAttachment]
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @AttachmentId VARCHAR(50),
    @AttachmentData NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @AttachmentIdKey VARCHAR(50) = CONCAT('"', @AttachmentId, '"')
    DECLARE @AttachmentIdPath VARCHAR(50) = CONCAT('$.', @AttachmentIdKey)

    UPDATE
        [dbo].[Cipher]
    SET
        [Attachments] = 
            CASE
            WHEN [Attachments] IS NULL THEN
                CONCAT('{', @AttachmentIdKey, ':', @AttachmentData, '}')
            ELSE
                JSON_MODIFY([Attachments], @AttachmentIdPath, JSON_QUERY(@AttachmentData, '$'))
            END
    WHERE
        [Id] = @Id

    IF @OrganizationId IS NOT NULL
    BEGIN
        EXEC [dbo].[Organization_UpdateStorage] @OrganizationId
        EXEC [dbo].[User_BumpAccountRevisionDateByCipherId] @Id, @OrganizationId
    END
    ELSE IF @UserId IS NOT NULL
    BEGIN
        EXEC [dbo].[User_UpdateStorage] @UserId
        EXEC [dbo].[User_BumpAccountRevisionDate] @UserId
    END
END
GO
/****** Object:  StoredProcedure [dbo].[Cipher_UpdateCollections]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Cipher_UpdateCollections]
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @CollectionIds AS [dbo].[GuidIdArray] READONLY
AS
BEGIN
    SET NOCOUNT ON

    IF @OrganizationId IS NULL OR (SELECT COUNT(1) FROM @CollectionIds) < 1
    BEGIN
        RETURN(-1)
    END

    CREATE TABLE #AvailableCollections (
        [Id] UNIQUEIDENTIFIER
    )

    IF @UserId IS NULL
    BEGIN
        INSERT INTO #AvailableCollections
            SELECT
                [Id]
            FROM
                [dbo].[Collection]
            WHERE
                [OrganizationId] = @OrganizationId
    END
    ELSE
    BEGIN
        INSERT INTO #AvailableCollections
            SELECT
                C.[Id]
            FROM
                [dbo].[Collection] C
            INNER JOIN
                [Organization] O ON O.[Id] = C.[OrganizationId]
            INNER JOIN
                [dbo].[OrganizationUser] OU ON OU.[OrganizationId] = O.[Id] AND OU.[UserId] = @UserId
            LEFT JOIN
                [dbo].[CollectionUser] CU ON OU.[AccessAll] = 0 AND CU.[CollectionId] = C.[Id] AND CU.[OrganizationUserId] = OU.[Id]
            LEFT JOIN
                [dbo].[GroupUser] GU ON CU.[CollectionId] IS NULL AND OU.[AccessAll] = 0 AND GU.[OrganizationUserId] = OU.[Id]
            LEFT JOIN
                [dbo].[Group] G ON G.[Id] = GU.[GroupId]
            LEFT JOIN
                [dbo].[CollectionGroup] CG ON G.[AccessAll] = 0 AND CG.[CollectionId] = C.[Id] AND CG.[GroupId] = GU.[GroupId]
            WHERE
                O.[Id] = @OrganizationId
                AND O.[Enabled] = 1
                AND OU.[Status] = 2 -- Confirmed
                AND (
                    OU.[AccessAll] = 1
                    OR CU.[ReadOnly] = 0
                    OR G.[AccessAll] = 1
                    OR CG.[ReadOnly] = 0
                )
    END

    IF (SELECT COUNT(1) FROM #AvailableCollections) < 1
    BEGIN
        -- No writable collections available to share with in this organization.
        RETURN(-1)
    END

    INSERT INTO [dbo].[CollectionCipher]
    (
        [CollectionId],
        [CipherId]
    )
    SELECT
        [Id],
        @Id
    FROM
        @CollectionIds
    WHERE
        [Id] IN (SELECT [Id] FROM #AvailableCollections)

    RETURN(0)
END
GO
/****** Object:  StoredProcedure [dbo].[Cipher_UpdatePartial]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Cipher_UpdatePartial]
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @FolderId UNIQUEIDENTIFIER,
    @Favorite BIT
AS
BEGIN
    SET NOCOUNT ON
    
    DECLARE @UserIdKey VARCHAR(50) = CONCAT('"', @UserId, '"')
    DECLARE @UserIdPath VARCHAR(50) = CONCAT('$.', @UserIdKey)

    UPDATE
        [dbo].[Cipher]
    SET
        [Folders] = 
            CASE
            WHEN @FolderId IS NOT NULL AND [Folders] IS NULL THEN
                CONCAT('{', @UserIdKey, ':"', @FolderId, '"', '}')
            WHEN @FolderId IS NOT NULL THEN
                JSON_MODIFY([Folders], @UserIdPath, CAST(@FolderId AS VARCHAR(50)))
            ELSE
                JSON_MODIFY([Folders], @UserIdPath, NULL)
            END,
        [Favorites] =
            CASE
            WHEN @Favorite = 1 AND [Favorites] IS NULL THEN
                CONCAT('{', @UserIdKey, ':true}')
            WHEN @Favorite = 1 THEN
                JSON_MODIFY([Favorites], @UserIdPath, CAST(1 AS BIT))
            ELSE
                JSON_MODIFY([Favorites], @UserIdPath, NULL)
            END
    WHERE
        [Id] = @Id

    IF @UserId IS NOT NULL
    BEGIN
        EXEC [dbo].[User_BumpAccountRevisionDate] @UserId
    END
END
GO
/****** Object:  StoredProcedure [dbo].[Cipher_UpdateWithCollections]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Cipher_UpdateWithCollections]
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @Type TINYINT,
    @Data NVARCHAR(MAX),
    @Favorites NVARCHAR(MAX),
    @Folders NVARCHAR(MAX),
    @Attachments NVARCHAR(MAX),
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7),
    @DeletedDate DATETIME2(7),
    @CollectionIds AS [dbo].[GuidIdArray] READONLY
AS
BEGIN
    SET NOCOUNT ON

    BEGIN TRANSACTION Cipher_UpdateWithCollections

    DECLARE @UpdateCollectionsSuccess INT
    EXEC @UpdateCollectionsSuccess = [dbo].[Cipher_UpdateCollections] @Id, @UserId, @OrganizationId, @CollectionIds

    IF @UpdateCollectionsSuccess < 0
    BEGIN
        COMMIT TRANSACTION Cipher_UpdateWithCollections
        SELECT -1 -- -1 = Failure
        RETURN
    END

    UPDATE
        [dbo].[Cipher]
    SET
        [UserId] = NULL,
        [OrganizationId] = @OrganizationId,
        [Data] = @Data,
        [Attachments] = @Attachments,
        [RevisionDate] = @RevisionDate,
        [DeletedDate] = @DeletedDate
        -- No need to update CreationDate, Favorites, Folders, or Type since that data will not change
    WHERE
        [Id] = @Id

    COMMIT TRANSACTION Cipher_UpdateWithCollections

    IF @Attachments IS NOT NULL
    BEGIN
        EXEC [dbo].[Organization_UpdateStorage] @OrganizationId
        EXEC [dbo].[User_UpdateStorage] @UserId
    END

    EXEC [dbo].[User_BumpAccountRevisionDateByCipherId] @Id, @OrganizationId

    SELECT 0 -- 0 = Success
END
GO
/****** Object:  StoredProcedure [dbo].[CipherDetails_Create]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CipherDetails_Create]
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @Type TINYINT,
    @Data NVARCHAR(MAX),
    @Favorites NVARCHAR(MAX), -- not used
    @Folders NVARCHAR(MAX), -- not used
    @Attachments NVARCHAR(MAX), -- not used
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7),
    @FolderId UNIQUEIDENTIFIER,
    @Favorite BIT,
    @Edit BIT, -- not used
    @ViewPassword BIT, -- not used
    @OrganizationUseTotp BIT, -- not used
    @DeletedDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @UserIdKey VARCHAR(50) = CONCAT('"', @UserId, '"')
    DECLARE @UserIdPath VARCHAR(50) = CONCAT('$.', @UserIdKey)

    INSERT INTO [dbo].[Cipher]
    (
        [Id],
        [UserId],
        [OrganizationId],
        [Type],
        [Data],
        [Favorites],
        [Folders],
        [CreationDate],
        [RevisionDate],
        [DeletedDate]
    )
    VALUES
    (
        @Id,
        CASE WHEN @OrganizationId IS NULL THEN @UserId ELSE NULL END,
        @OrganizationId,
        @Type,
        @Data,
        CASE WHEN @Favorite = 1 THEN CONCAT('{', @UserIdKey, ':true}') ELSE NULL END,
        CASE WHEN @FolderId IS NOT NULL THEN CONCAT('{', @UserIdKey, ':"', @FolderId, '"', '}') ELSE NULL END,
        @CreationDate,
        @RevisionDate,
        @DeletedDate
    )

    IF @OrganizationId IS NOT NULL
    BEGIN
        EXEC [dbo].[User_BumpAccountRevisionDateByCipherId] @Id, @OrganizationId
    END
    ELSE IF @UserId IS NOT NULL
    BEGIN
        EXEC [dbo].[User_BumpAccountRevisionDate] @UserId
    END
END
GO
/****** Object:  StoredProcedure [dbo].[CipherDetails_CreateWithCollections]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CipherDetails_CreateWithCollections]
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @Type TINYINT,
    @Data NVARCHAR(MAX),
    @Favorites NVARCHAR(MAX), -- not used
    @Folders NVARCHAR(MAX), -- not used
    @Attachments NVARCHAR(MAX), -- not used
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7),
    @FolderId UNIQUEIDENTIFIER,
    @Favorite BIT,
    @Edit BIT, -- not used
    @ViewPassword BIT, -- not used
    @OrganizationUseTotp BIT, -- not used
    @DeletedDate DATETIME2(7),
    @CollectionIds AS [dbo].[GuidIdArray] READONLY
AS
BEGIN
    SET NOCOUNT ON

    EXEC [dbo].[CipherDetails_Create] @Id, @UserId, @OrganizationId, @Type, @Data, @Favorites, @Folders,
        @Attachments, @CreationDate, @RevisionDate, @FolderId, @Favorite, @Edit, @ViewPassword,
        @OrganizationUseTotp, @DeletedDate

    DECLARE @UpdateCollectionsSuccess INT
    EXEC @UpdateCollectionsSuccess = [dbo].[Cipher_UpdateCollections] @Id, @UserId, @OrganizationId, @CollectionIds
END
GO
/****** Object:  StoredProcedure [dbo].[CipherDetails_ReadByIdUserId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CipherDetails_ReadByIdUserId]
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT TOP 1
        *
    FROM
        [dbo].[UserCipherDetails](@UserId)
    WHERE
        [Id] = @Id
    ORDER BY
        [Edit] DESC
END
GO
/****** Object:  StoredProcedure [dbo].[CipherDetails_ReadByUserId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CipherDetails_ReadByUserId]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[UserCipherDetails](@UserId)
END
GO
/****** Object:  StoredProcedure [dbo].[CipherDetails_ReadWithoutOrganizationsByUserId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CipherDetails_ReadWithoutOrganizationsByUserId]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *,
        1 [Edit],
        1 [ViewPassword],
        0 [OrganizationUseTotp]
    FROM
        [dbo].[CipherDetails](@UserId)
    WHERE
        [UserId] = @UserId
END
GO
/****** Object:  StoredProcedure [dbo].[CipherDetails_Update]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CipherDetails_Update]
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @Type TINYINT,
    @Data NVARCHAR(MAX),
    @Favorites NVARCHAR(MAX), -- not used
    @Folders NVARCHAR(MAX), -- not used
    @Attachments NVARCHAR(MAX), -- not used
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7),
    @FolderId UNIQUEIDENTIFIER,
    @Favorite BIT,
    @Edit BIT, -- not used
    @ViewPassword BIT, -- not used
    @OrganizationUseTotp BIT, -- not used
    @DeletedDate DATETIME2(2)
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @UserIdKey VARCHAR(50) = CONCAT('"', @UserId, '"')
    DECLARE @UserIdPath VARCHAR(50) = CONCAT('$.', @UserIdKey)

    UPDATE
        [dbo].[Cipher]
    SET
        [UserId] = CASE WHEN @OrganizationId IS NULL THEN @UserId ELSE NULL END,
        [OrganizationId] = @OrganizationId,
        [Type] = @Type,
        [Data] = @Data,
        [Folders] = 
            CASE
            WHEN @FolderId IS NOT NULL AND [Folders] IS NULL THEN
                CONCAT('{', @UserIdKey, ':"', @FolderId, '"', '}')
            WHEN @FolderId IS NOT NULL THEN
                JSON_MODIFY([Folders], @UserIdPath, CAST(@FolderId AS VARCHAR(50)))
            ELSE
                JSON_MODIFY([Folders], @UserIdPath, NULL)
            END,
        [Favorites] =
            CASE
            WHEN @Favorite = 1 AND [Favorites] IS NULL THEN
                CONCAT('{', @UserIdKey, ':true}')
            WHEN @Favorite = 1 THEN
                JSON_MODIFY([Favorites], @UserIdPath, CAST(1 AS BIT))
            ELSE
                JSON_MODIFY([Favorites], @UserIdPath, NULL)
            END,
        [CreationDate] = @CreationDate,
        [RevisionDate] = @RevisionDate,
        [DeletedDate] = @DeletedDate
    WHERE
        [Id] = @Id

    IF @OrganizationId IS NOT NULL
    BEGIN
        EXEC [dbo].[User_BumpAccountRevisionDateByCipherId] @Id, @OrganizationId
    END
    ELSE IF @UserId IS NOT NULL
    BEGIN
        EXEC [dbo].[User_BumpAccountRevisionDate] @UserId
    END
END
GO
/****** Object:  StoredProcedure [dbo].[CipherOrganizationDetails_ReadById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CipherOrganizationDetails_ReadById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        C.*,
        CASE 
            WHEN O.[UseTotp] = 1 THEN 1
            ELSE 0
        END [OrganizationUseTotp]
    FROM
        [dbo].[CipherView] C
    LEFT JOIN
        [dbo].[Organization] O ON O.[Id] = C.[OrganizationId]
    WHERE
        C.[Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[Collection_Create]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Collection_Create]
    @Id UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @Name VARCHAR(MAX),
    @ExternalId NVARCHAR(300),
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [dbo].[Collection]
    (
        [Id],
        [OrganizationId],
        [Name],
        [ExternalId],
        [CreationDate],
        [RevisionDate]
    )
    VALUES
    (
        @Id,
        @OrganizationId,
        @Name,
        @ExternalId,
        @CreationDate,
        @RevisionDate
    )

    EXEC [dbo].[User_BumpAccountRevisionDateByCollectionId] @Id, @OrganizationId
END
GO
/****** Object:  StoredProcedure [dbo].[Collection_CreateWithGroups]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Collection_CreateWithGroups]
    @Id UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @Name VARCHAR(MAX),
    @ExternalId NVARCHAR(300),
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7),
    @Groups AS [dbo].[SelectionReadOnlyArray] READONLY
AS
BEGIN
    SET NOCOUNT ON

    EXEC [dbo].[Collection_Create] @Id, @OrganizationId, @Name, @ExternalId, @CreationDate, @RevisionDate

    ;WITH [AvailableGroupsCTE] AS(
        SELECT
            [Id]
        FROM
            [dbo].[Group]
        WHERE
            [OrganizationId] = @OrganizationId
    )
    INSERT INTO [dbo].[CollectionGroup]
    (
        [CollectionId],
        [GroupId],
        [ReadOnly],
        [HidePasswords]
    )
    SELECT
        @Id,
        [Id],
        [ReadOnly],
        [HidePasswords]
    FROM
        @Groups
    WHERE
        [Id] IN (SELECT [Id] FROM [AvailableGroupsCTE])

    EXEC [dbo].[User_BumpAccountRevisionDateByOrganizationId] @OrganizationId
END
GO
/****** Object:  StoredProcedure [dbo].[Collection_DeleteById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Collection_DeleteById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @OrganizationId UNIQUEIDENTIFIER = (SELECT TOP 1 [OrganizationId] FROM [dbo].[Collection] WHERE [Id] = @Id)
    IF @OrganizationId IS NOT NULL
    BEGIN
        EXEC [dbo].[User_BumpAccountRevisionDateByCollectionId] @Id, @OrganizationId
    END

    DELETE
    FROM
        [dbo].[CollectionGroup]
    WHERE
        [CollectionId] = @Id

    DELETE
    FROM
        [dbo].[Collection]
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[Collection_ReadById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Collection_ReadById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[CollectionView]
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[Collection_ReadByIdUserId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Collection_ReadByIdUserId]
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON
    SELECT TOP 1
        *
    FROM
        [dbo].[UserCollectionDetails](@UserId)
    WHERE
        [Id] = @Id
    ORDER BY
        [ReadOnly] ASC
END
GO
/****** Object:  StoredProcedure [dbo].[Collection_ReadByOrganizationId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Collection_ReadByOrganizationId]
    @OrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[CollectionView]
    WHERE
        [OrganizationId] = @OrganizationId
END
GO
/****** Object:  StoredProcedure [dbo].[Collection_ReadByUserId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Collection_ReadByUserId]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[UserCollectionDetails](@UserId)
END
GO
/****** Object:  StoredProcedure [dbo].[Collection_ReadCountByOrganizationId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Collection_ReadCountByOrganizationId]
    @OrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        COUNT(1)
    FROM
        [dbo].[Collection]
    WHERE
        [OrganizationId] = @OrganizationId
END
GO
/****** Object:  StoredProcedure [dbo].[Collection_ReadWithGroupsById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Collection_ReadWithGroupsById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    EXEC [dbo].[Collection_ReadById] @Id

    SELECT
        [GroupId] [Id],
        [ReadOnly],
        [HidePasswords]
    FROM
        [dbo].[CollectionGroup]
    WHERE
        [CollectionId] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[Collection_ReadWithGroupsByIdUserId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Collection_ReadWithGroupsByIdUserId]
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    EXEC [dbo].[Collection_ReadByIdUserId] @Id, @UserId

    SELECT
        [GroupId] [Id],
        [ReadOnly],
        [HidePasswords]
    FROM
        [dbo].[CollectionGroup]
    WHERE
        [CollectionId] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[Collection_Update]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Collection_Update]
    @Id UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @Name VARCHAR(MAX),
    @ExternalId NVARCHAR(300),
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    UPDATE
        [dbo].[Collection]
    SET
        [OrganizationId] = @OrganizationId,
        [Name] = @Name,
        [ExternalId] = @ExternalId,
        [CreationDate] = @CreationDate,
        [RevisionDate] = @RevisionDate
    WHERE
        [Id] = @Id

    EXEC [dbo].[User_BumpAccountRevisionDateByCollectionId] @Id, @OrganizationId
END
GO
/****** Object:  StoredProcedure [dbo].[Collection_UpdateWithGroups]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Collection_UpdateWithGroups]
    @Id UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @Name VARCHAR(MAX),
    @ExternalId NVARCHAR(300),
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7),
    @Groups AS [dbo].[SelectionReadOnlyArray] READONLY
AS
BEGIN
    SET NOCOUNT ON

    EXEC [dbo].[Collection_Update] @Id, @OrganizationId, @Name, @ExternalId, @CreationDate, @RevisionDate

    ;WITH [AvailableGroupsCTE] AS(
        SELECT
            Id
        FROM
            [dbo].[Group]
        WHERE
            OrganizationId = @OrganizationId
    )
    MERGE
        [dbo].[CollectionGroup] AS [Target]
    USING 
        @Groups AS [Source]
    ON
        [Target].[CollectionId] = @Id
        AND [Target].[GroupId] = [Source].[Id]
    WHEN NOT MATCHED BY TARGET
    AND [Source].[Id] IN (SELECT [Id] FROM [AvailableGroupsCTE]) THEN
        INSERT VALUES
        (
            @Id,
            [Source].[Id],
            [Source].[ReadOnly],
            [Source].[HidePasswords]
        )
    WHEN MATCHED AND (
        [Target].[ReadOnly] != [Source].[ReadOnly]
        OR [Target].[HidePasswords] != [Source].[HidePasswords]
    ) THEN
        UPDATE SET [Target].[ReadOnly] = [Source].[ReadOnly],
                   [Target].[HidePasswords] = [Source].[HidePasswords]
    WHEN NOT MATCHED BY SOURCE
    AND [Target].[CollectionId] = @Id THEN
        DELETE
    ;

    EXEC [dbo].[User_BumpAccountRevisionDateByCollectionId] @Id, @OrganizationId
END
GO
/****** Object:  StoredProcedure [dbo].[CollectionCipher_Create]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CollectionCipher_Create]
    @CollectionId UNIQUEIDENTIFIER,
    @CipherId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [dbo].[CollectionCipher]
    (
        [CollectionId],
        [CipherId]
    )
    VALUES
    (
        @CollectionId,
        @CipherId
    )

    DECLARE @OrganizationId UNIQUEIDENTIFIER = (SELECT TOP 1 [OrganizationId] FROM [dbo].[Cipher] WHERE [Id] = @CipherId)
    IF @OrganizationId IS NOT NULL
    BEGIN
        EXEC [dbo].[User_BumpAccountRevisionDateByCollectionId] @CollectionId, @OrganizationId
    END
END
GO
/****** Object:  StoredProcedure [dbo].[CollectionCipher_Delete]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CollectionCipher_Delete]
    @CollectionId UNIQUEIDENTIFIER,
    @CipherId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    DELETE
    FROM
        [dbo].[CollectionCipher]
    WHERE
        [CollectionId] = @CollectionId
        AND [CipherId] = @CipherId

    DECLARE @OrganizationId UNIQUEIDENTIFIER = (SELECT TOP 1 [OrganizationId] FROM [dbo].[Cipher] WHERE [Id] = @CipherId)
    IF @OrganizationId IS NOT NULL
    BEGIN
        EXEC [dbo].[User_BumpAccountRevisionDateByCollectionId] @CollectionId, @OrganizationId
    END
END
GO
/****** Object:  StoredProcedure [dbo].[CollectionCipher_ReadByOrganizationId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CollectionCipher_ReadByOrganizationId]
    @OrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        SC.*
    FROM
        [dbo].[CollectionCipher] SC
    INNER JOIN
        [dbo].[Collection] S ON S.[Id] = SC.[CollectionId]
    WHERE
        S.[OrganizationId] = @OrganizationId
END
GO
/****** Object:  StoredProcedure [dbo].[CollectionCipher_ReadByUserId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CollectionCipher_ReadByUserId]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        CC.*
    FROM
        [dbo].[CollectionCipher] CC
    INNER JOIN
        [dbo].[Collection] S ON S.[Id] = CC.[CollectionId]
    INNER JOIN
        [dbo].[OrganizationUser] OU ON OU.[OrganizationId] = S.[OrganizationId] AND OU.[UserId] = @UserId
    LEFT JOIN
        [dbo].[CollectionUser] CU ON OU.[AccessAll] = 0 AND CU.[CollectionId] = S.[Id] AND CU.[OrganizationUserId] = OU.[Id]
    LEFT JOIN
        [dbo].[GroupUser] GU ON CU.[CollectionId] IS NULL AND OU.[AccessAll] = 0 AND GU.[OrganizationUserId] = OU.[Id]
    LEFT JOIN
        [dbo].[Group] G ON G.[Id] = GU.[GroupId]
    LEFT JOIN
        [dbo].[CollectionGroup] CG ON G.[AccessAll] = 0 AND CG.[CollectionId] = CC.[CollectionId] AND CG.[GroupId] = GU.[GroupId]
    WHERE
        OU.[Status] = 2 -- Confirmed
        AND (
            OU.[AccessAll] = 1
            OR CU.[CollectionId] IS NOT NULL
            OR G.[AccessAll] = 1
            OR CG.[CollectionId] IS NOT NULL
        )
END
GO
/****** Object:  StoredProcedure [dbo].[CollectionCipher_ReadByUserIdCipherId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CollectionCipher_ReadByUserIdCipherId]
    @UserId UNIQUEIDENTIFIER,
    @CipherId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        CC.*
    FROM
        [dbo].[CollectionCipher] CC
    INNER JOIN
        [dbo].[Collection] S ON S.[Id] = CC.[CollectionId]
    INNER JOIN
        [dbo].[OrganizationUser] OU ON OU.[OrganizationId] = S.[OrganizationId] AND OU.[UserId] = @UserId
    LEFT JOIN
        [dbo].[CollectionUser] CU ON OU.[AccessAll] = 0 AND CU.[CollectionId] = S.[Id] AND CU.[OrganizationUserId] = OU.[Id]
    LEFT JOIN
        [dbo].[GroupUser] GU ON CU.[CollectionId] IS NULL AND OU.[AccessAll] = 0 AND GU.[OrganizationUserId] = OU.[Id]
    LEFT JOIN
        [dbo].[Group] G ON G.[Id] = GU.[GroupId]
    LEFT JOIN
        [dbo].[CollectionGroup] CG ON G.[AccessAll] = 0 AND CG.[CollectionId] = CC.[CollectionId] AND CG.[GroupId] = GU.[GroupId]
    WHERE
        CC.[CipherId] = @CipherId
        AND OU.[Status] = 2 -- Confirmed
        AND (
            OU.[AccessAll] = 1
            OR CU.[CollectionId] IS NOT NULL
            OR G.[AccessAll] = 1
            OR CG.[CollectionId] IS NOT NULL
        )
END
GO
/****** Object:  StoredProcedure [dbo].[CollectionCipher_UpdateCollections]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CollectionCipher_UpdateCollections]
    @CipherId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @CollectionIds AS [dbo].[GuidIdArray] READONLY
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @OrgId UNIQUEIDENTIFIER = (
        SELECT TOP 1
            [OrganizationId]
        FROM
            [dbo].[Cipher]
        WHERE
            [Id] = @CipherId
    )

    ;WITH [AvailableCollectionsCTE] AS(
        SELECT
            C.[Id]
        FROM
            [dbo].[Collection] C
        INNER JOIN
            [Organization] O ON O.[Id] = C.[OrganizationId]
        INNER JOIN
            [dbo].[OrganizationUser] OU ON OU.[OrganizationId] = O.[Id] AND OU.[UserId] = @UserId
        LEFT JOIN
            [dbo].[CollectionUser] CU ON OU.[AccessAll] = 0 AND CU.[CollectionId] = C.[Id] AND CU.[OrganizationUserId] = OU.[Id]
        LEFT JOIN
            [dbo].[GroupUser] GU ON CU.[CollectionId] IS NULL AND OU.[AccessAll] = 0 AND GU.[OrganizationUserId] = OU.[Id]
        LEFT JOIN
            [dbo].[Group] G ON G.[Id] = GU.[GroupId]
        LEFT JOIN
            [dbo].[CollectionGroup] CG ON G.[AccessAll] = 0 AND CG.[CollectionId] = C.[Id] AND CG.[GroupId] = GU.[GroupId]
        WHERE
            O.[Id] = @OrgId
            AND O.[Enabled] = 1
            AND OU.[Status] = 2 -- Confirmed
            AND (
                OU.[AccessAll] = 1
                OR CU.[ReadOnly] = 0
                OR G.[AccessAll] = 1
                OR CG.[ReadOnly] = 0
            )
    ),
    [CollectionCiphersCTE] AS(
        SELECT
            [CollectionId],
            [CipherId]
        FROM
            [dbo].[CollectionCipher]
        WHERE
            [CipherId] = @CipherId
    )
    MERGE
        [CollectionCiphersCTE] AS [Target]
    USING 
        @CollectionIds AS [Source]
    ON
        [Target].[CollectionId] = [Source].[Id]
        AND [Target].[CipherId] = @CipherId
    WHEN NOT MATCHED BY TARGET
    AND [Source].[Id] IN (SELECT [Id] FROM [AvailableCollectionsCTE]) THEN
        INSERT VALUES
        (
            [Source].[Id],
            @CipherId
        )
    WHEN NOT MATCHED BY SOURCE
    AND [Target].[CipherId] = @CipherId
    AND [Target].[CollectionId] IN (SELECT [Id] FROM [AvailableCollectionsCTE]) THEN
        DELETE
    ;

    IF @OrgId IS NOT NULL
    BEGIN
        EXEC [dbo].[User_BumpAccountRevisionDateByOrganizationId] @OrgId
    END
END
GO
/****** Object:  StoredProcedure [dbo].[CollectionCipher_UpdateCollectionsAdmin]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CollectionCipher_UpdateCollectionsAdmin]
    @CipherId UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @CollectionIds AS [dbo].[GuidIdArray] READONLY
AS
BEGIN
    SET NOCOUNT ON

    ;WITH [AvailableCollectionsCTE] AS(
        SELECT
            Id
        FROM
            [dbo].[Collection]
        WHERE
            OrganizationId = @OrganizationId
    ),
    [CollectionCiphersCTE] AS(
        SELECT
            [CollectionId],
            [CipherId]
        FROM
            [dbo].[CollectionCipher]
        WHERE
            [CipherId] = @CipherId
    )
    MERGE
        [CollectionCiphersCTE] AS [Target]
    USING 
        @CollectionIds AS [Source]
    ON
        [Target].[CollectionId] = [Source].[Id]
        AND [Target].[CipherId] = @CipherId
    WHEN NOT MATCHED BY TARGET
    AND [Source].[Id] IN (SELECT [Id] FROM [AvailableCollectionsCTE]) THEN
        INSERT VALUES
        (
            [Source].[Id],
            @CipherId
        )
    WHEN NOT MATCHED BY SOURCE
    AND [Target].[CipherId] = @CipherId THEN
        DELETE
    ;

    IF @OrganizationId IS NOT NULL
    BEGIN
        EXEC [dbo].[User_BumpAccountRevisionDateByOrganizationId] @OrganizationId
    END
END
GO
/****** Object:  StoredProcedure [dbo].[CollectionCipher_UpdateCollectionsForCiphers]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CollectionCipher_UpdateCollectionsForCiphers]
    @CipherIds AS [dbo].[GuidIdArray] READONLY,
    @OrganizationId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @CollectionIds AS [dbo].[GuidIdArray] READONLY
AS
BEGIN
    SET NOCOUNT ON

    CREATE TABLE #AvailableCollections (
        [Id] UNIQUEIDENTIFIER
    )

    INSERT INTO #AvailableCollections
        SELECT
            C.[Id]
        FROM
            [dbo].[Collection] C
        INNER JOIN
            [Organization] O ON O.[Id] = C.[OrganizationId]
        INNER JOIN
            [dbo].[OrganizationUser] OU ON OU.[OrganizationId] = O.[Id] AND OU.[UserId] = @UserId
        LEFT JOIN
            [dbo].[CollectionUser] CU ON OU.[AccessAll] = 0 AND CU.[CollectionId] = C.[Id] AND CU.[OrganizationUserId] = OU.[Id]
        LEFT JOIN
            [dbo].[GroupUser] GU ON CU.[CollectionId] IS NULL AND OU.[AccessAll] = 0 AND GU.[OrganizationUserId] = OU.[Id]
        LEFT JOIN
            [dbo].[Group] G ON G.[Id] = GU.[GroupId]
        LEFT JOIN
            [dbo].[CollectionGroup] CG ON G.[AccessAll] = 0 AND CG.[CollectionId] = C.[Id] AND CG.[GroupId] = GU.[GroupId]
        WHERE
            O.[Id] = @OrganizationId
            AND O.[Enabled] = 1
            AND OU.[Status] = 2 -- Confirmed
            AND (
                OU.[AccessAll] = 1
                OR CU.[ReadOnly] = 0
                OR G.[AccessAll] = 1
                OR CG.[ReadOnly] = 0
            )

    IF (SELECT COUNT(1) FROM #AvailableCollections) < 1
    BEGIN
        -- No writable collections available to share with in this organization.
        RETURN
    END

    INSERT INTO [dbo].[CollectionCipher]
    (
        [CollectionId],
        [CipherId]
    )
    SELECT 
        [Collection].[Id],
        [Cipher].[Id]
    FROM
        @CollectionIds [Collection]
    INNER JOIN
        @CipherIds [Cipher] ON 1 = 1
    WHERE
        [Collection].[Id] IN (SELECT [Id] FROM #AvailableCollections)

    EXEC [dbo].[User_BumpAccountRevisionDateByOrganizationId] @OrganizationId
END
GO
/****** Object:  StoredProcedure [dbo].[CollectionUser_Delete]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CollectionUser_Delete]
    @CollectionId UNIQUEIDENTIFIER,
    @OrganizationUserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    DELETE
    FROM
        [dbo].[CollectionUser]
    WHERE
        [CollectionId] = @CollectionId
        AND [OrganizationUserId] = @OrganizationUserId

    EXEC [dbo].[User_BumpAccountRevisionDateByOrganizationUserId] @OrganizationUserId
END
GO
/****** Object:  StoredProcedure [dbo].[CollectionUser_ReadByCollectionId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CollectionUser_ReadByCollectionId]
    @CollectionId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        [OrganizationUserId] [Id],
        [ReadOnly],
        [HidePasswords]
    FROM
        [dbo].[CollectionUser]
    WHERE
        [CollectionId] = @CollectionId
END
GO
/****** Object:  StoredProcedure [dbo].[CollectionUser_UpdateUsers]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CollectionUser_UpdateUsers]
    @CollectionId UNIQUEIDENTIFIER,
    @Users AS [dbo].[SelectionReadOnlyArray] READONLY
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @OrgId UNIQUEIDENTIFIER = (
        SELECT TOP 1
            [OrganizationId]
        FROM
            [dbo].[Collection]
        WHERE
            [Id] = @CollectionId
    )

    -- Update
    UPDATE
        [Target]
    SET
        [Target].[ReadOnly] = [Source].[ReadOnly],
        [Target].[HidePasswords] = [Source].[HidePasswords]
    FROM
        [dbo].[CollectionUser] [Target]
    INNER JOIN
        @Users [Source] ON [Source].[Id] = [Target].[OrganizationUserId]
    WHERE
        [Target].[CollectionId] = @CollectionId
        AND (
            [Target].[ReadOnly] != [Source].[ReadOnly]
            OR [Target].[HidePasswords] != [Source].[HidePasswords]
        )

    -- Insert
    INSERT INTO
        [dbo].[CollectionUser]
    SELECT
        @CollectionId,
        [Source].[Id],
        [Source].[ReadOnly],
        [Source].[HidePasswords]
    FROM
        @Users [Source]
    INNER JOIN
        [dbo].[OrganizationUser] OU ON [Source].[Id] = OU.[Id] AND OU.[OrganizationId] = @OrgId
    WHERE
        NOT EXISTS (
            SELECT
                1
            FROM
                [dbo].[CollectionUser]
            WHERE
                [CollectionId] = @CollectionId
                AND [OrganizationUserId] = [Source].[Id]
        )
    
    -- Delete
    DELETE
        CU
    FROM
        [dbo].[CollectionUser] CU
    WHERE
        CU.[CollectionId] = @CollectionId
        AND NOT EXISTS (
            SELECT
                1
            FROM
                @Users
            WHERE
                [Id] = CU.[OrganizationUserId]
        )

    EXEC [dbo].[User_BumpAccountRevisionDateByCollectionId] @CollectionId, @OrgId
END
GO
/****** Object:  StoredProcedure [dbo].[Device_ClearPushTokenById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Device_ClearPushTokenById]
    @Id NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON

    UPDATE
        [dbo].[Device]
    SET
        [PushToken] = NULL
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[Device_Create]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Device_Create]
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @Name NVARCHAR(50),
    @Type TINYINT,
    @Identifier NVARCHAR(50),
    @PushToken NVARCHAR(255),
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [dbo].[Device]
    (
        [Id],
        [UserId],
        [Name],
        [Type],
        [Identifier],
        [PushToken],
        [CreationDate],
        [RevisionDate]
    )
    VALUES
    (
        @Id,
        @UserId,
        @Name,
        @Type,
        @Identifier,
        @PushToken,
        @CreationDate,
        @RevisionDate
    )
END
GO
/****** Object:  StoredProcedure [dbo].[Device_DeleteById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Device_DeleteById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    DELETE
    FROM
        [dbo].[Device]
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[Device_ReadById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Device_ReadById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[DeviceView]
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[Device_ReadByIdentifier]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Device_ReadByIdentifier]
    @Identifier NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[DeviceView]
    WHERE
        [Identifier] = @Identifier
END
GO
/****** Object:  StoredProcedure [dbo].[Device_ReadByIdentifierUserId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Device_ReadByIdentifierUserId]
    @UserId UNIQUEIDENTIFIER,
    @Identifier NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[DeviceView]
    WHERE
        [UserId] = @UserId
        AND [Identifier] = @Identifier
END
GO
/****** Object:  StoredProcedure [dbo].[Device_ReadByUserId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Device_ReadByUserId]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[DeviceView]
    WHERE
        [UserId] = @UserId
END
GO
/****** Object:  StoredProcedure [dbo].[Device_Update]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Device_Update]
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @Name NVARCHAR(50),
    @Type TINYINT,
    @Identifier NVARCHAR(50),
    @PushToken NVARCHAR(255),
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    UPDATE
        [dbo].[Device]
    SET
        [UserId] = @UserId,
        [Name] = @Name,
        [Type] = @Type,
        [Identifier] = @Identifier,
        [PushToken] = @PushToken,
        [CreationDate] = @CreationDate,
        [RevisionDate] = @RevisionDate
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[EmergencyAccess_Create]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[EmergencyAccess_Create]
    @Id UNIQUEIDENTIFIER,
    @GrantorId UNIQUEIDENTIFIER,
    @GranteeId UNIQUEIDENTIFIER,
    @Email NVARCHAR(50),
    @KeyEncrypted VARCHAR(MAX),
    @Type TINYINT,
    @Status TINYINT,
    @WaitTimeDays SMALLINT,
    @RecoveryInitiatedDate DATETIME2(7),
    @LastNotificationDate DATETIME2(7),
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [dbo].[EmergencyAccess]
    (
        [Id],
        [GrantorId],
        [GranteeId],
        [Email],
        [KeyEncrypted],
        [Type],
        [Status],
        [WaitTimeDays],
        [RecoveryInitiatedDate],
        [LastNotificationDate],
        [CreationDate],
        [RevisionDate]
    )
    VALUES
    (
        @Id,
        @GrantorId,
        @GranteeId,
        @Email,
        @KeyEncrypted,
        @Type,
        @Status,
        @WaitTimeDays,
        @RecoveryInitiatedDate,
        @LastNotificationDate,
        @CreationDate,
        @RevisionDate
    )
END
GO
/****** Object:  StoredProcedure [dbo].[EmergencyAccess_DeleteById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[EmergencyAccess_DeleteById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON
    
    EXEC [dbo].[User_BumpAccountRevisionDateByEmergencyAccessGranteeId] @Id
    
    DELETE
    FROM
        [dbo].[EmergencyAccess]
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[EmergencyAccess_ReadById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[EmergencyAccess_ReadById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[EmergencyAccess]
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[EmergencyAccess_ReadCountByGrantorIdEmail]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[EmergencyAccess_ReadCountByGrantorIdEmail]
    @GrantorId UNIQUEIDENTIFIER,
    @Email NVARCHAR(50),
    @OnlyUsers BIT
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        COUNT(1)
    FROM
        [dbo].[EmergencyAccess] EA
    LEFT JOIN
        [dbo].[User] U ON EA.[GranteeId] = U.[Id]
    WHERE
        EA.[GrantorId] = @GrantorId
        AND (
            (@OnlyUsers = 0 AND (EA.[Email] = @Email OR U.[Email] = @Email))
            OR (@OnlyUsers = 1 AND U.[Email] = @Email)
        )
END
GO
/****** Object:  StoredProcedure [dbo].[EmergencyAccess_ReadToNotify]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[EmergencyAccess_ReadToNotify]
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        EA.*,
        Grantee.Name as GranteeName,
        Grantor.Email as GrantorEmail
    FROM
        [dbo].[EmergencyAccess] EA
    LEFT JOIN
        [dbo].[User] Grantor ON Grantor.[Id] = EA.[GrantorId]
    LEFT JOIN
        [dbo].[User] Grantee On Grantee.[Id] = EA.[GranteeId]
    WHERE
        EA.[Status] = 3
    AND
        DATEADD(DAY, EA.[WaitTimeDays] - 1, EA.[RecoveryInitiatedDate]) <= GETUTCDATE()
    AND
        DATEADD(DAY, 1, EA.[LastNotificationDate]) <= GETUTCDATE()
END
GO
/****** Object:  StoredProcedure [dbo].[EmergencyAccess_Update]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[EmergencyAccess_Update]
    @Id UNIQUEIDENTIFIER,
    @GrantorId UNIQUEIDENTIFIER,
    @GranteeId UNIQUEIDENTIFIER,
    @Email NVARCHAR(50),
    @KeyEncrypted VARCHAR(MAX),
    @Type TINYINT,
    @Status TINYINT,
    @WaitTimeDays SMALLINT,
    @RecoveryInitiatedDate DATETIME2(7),
    @LastNotificationDate DATETIME2(7),
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    UPDATE
        [dbo].[EmergencyAccess]
    SET
        [GrantorId] = @GrantorId,
        [GranteeId] = @GranteeId,
        [Email] = @Email,
        [KeyEncrypted] = @KeyEncrypted,
        [Type] = @Type,
        [Status] = @Status,
        [WaitTimeDays] = @WaitTimeDays,
        [RecoveryInitiatedDate] = @RecoveryInitiatedDate,
        [LastNotificationDate] = @LastNotificationDate,
        [CreationDate] = @CreationDate,
        [RevisionDate] = @RevisionDate
    WHERE
        [Id] = @Id

    EXEC [dbo].[User_BumpAccountRevisionDate] @GranteeId
END
GO
/****** Object:  StoredProcedure [dbo].[EmergencyAccessDetails_ReadByGranteeId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[EmergencyAccessDetails_ReadByGranteeId]
    @GranteeId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[EmergencyAccessDetailsView]
    WHERE
        [GranteeId] = @GranteeId
END
GO
/****** Object:  StoredProcedure [dbo].[EmergencyAccessDetails_ReadByGrantorId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[EmergencyAccessDetails_ReadByGrantorId]
    @GrantorId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[EmergencyAccessDetailsView]
    WHERE
        [GrantorId] = @GrantorId
END
GO
/****** Object:  StoredProcedure [dbo].[EmergencyAccessDetails_ReadByIdGrantorId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[EmergencyAccessDetails_ReadByIdGrantorId]
    @Id UNIQUEIDENTIFIER,
    @GrantorId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[EmergencyAccessDetailsView]
    WHERE
        [Id] = @Id
    AND
        [GrantorId] = @GrantorId
END
GO
/****** Object:  StoredProcedure [dbo].[EmergencyAccessDetails_ReadExpiredRecoveries]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[EmergencyAccessDetails_ReadExpiredRecoveries]
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[EmergencyAccessDetailsView]
    WHERE
        [Status] = 3
    AND
        DATEADD(DAY, [WaitTimeDays], [RecoveryInitiatedDate]) <= GETUTCDATE()
END
GO
/****** Object:  StoredProcedure [dbo].[Event_Create]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Event_Create]
    @Id UNIQUEIDENTIFIER,
    @Type INT,
    @UserId UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @CipherId UNIQUEIDENTIFIER,
    @CollectionId UNIQUEIDENTIFIER,
    @PolicyId UNIQUEIDENTIFIER,
    @GroupId UNIQUEIDENTIFIER,
    @OrganizationUserId UNIQUEIDENTIFIER,
    @ActingUserId UNIQUEIDENTIFIER,
    @DeviceType SMALLINT,
    @IpAddress VARCHAR(50),
    @Date DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [dbo].[Event]
    (
        [Id],
        [Type],
        [UserId],
        [OrganizationId],
        [CipherId],
        [CollectionId],
        [PolicyId],
        [GroupId],
        [OrganizationUserId],
        [ActingUserId],
        [DeviceType],
        [IpAddress],
        [Date]
    )
    VALUES
    (
        @Id,
        @Type,
        @UserId,
        @OrganizationId,
        @CipherId,
        @CollectionId,
        @PolicyId,
        @GroupId,
        @OrganizationUserId,
        @ActingUserId,
        @DeviceType,
        @IpAddress,
        @Date
    )
END
GO
/****** Object:  StoredProcedure [dbo].[Event_ReadPageByCipherId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Event_ReadPageByCipherId]
    @OrganizationId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @CipherId UNIQUEIDENTIFIER,
    @StartDate DATETIME2(7),
    @EndDate DATETIME2(7),
    @BeforeDate DATETIME2(7),
    @PageSize INT
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[EventView]
    WHERE
        [Date] >= @StartDate
        AND (@BeforeDate IS NOT NULL OR [Date] <= @EndDate)
        AND (@BeforeDate IS NULL OR [Date] < @BeforeDate)
        AND (
            (@OrganizationId IS NULL AND [OrganizationId] IS NULL)
            OR (@OrganizationId IS NOT NULL AND [OrganizationId] = @OrganizationId)
        )
        AND (
            (@UserId IS NULL AND [UserId] IS NULL)
            OR (@UserId IS NOT NULL AND [UserId] = @UserId)
        )
        AND [CipherId]  = @CipherId
    ORDER BY [Date] DESC
    OFFSET 0 ROWS
    FETCH NEXT @PageSize ROWS ONLY
END
GO
/****** Object:  StoredProcedure [dbo].[Event_ReadPageByOrganizationId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Event_ReadPageByOrganizationId]
    @OrganizationId UNIQUEIDENTIFIER,
    @StartDate DATETIME2(7),
    @EndDate DATETIME2(7),
    @BeforeDate DATETIME2(7),
    @PageSize INT
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[EventView]
    WHERE
        [Date] >= @StartDate
        AND (@BeforeDate IS NOT NULL OR [Date] <= @EndDate)
        AND (@BeforeDate IS NULL OR [Date] < @BeforeDate)
        AND [OrganizationId] = @OrganizationId
    ORDER BY [Date] DESC
    OFFSET 0 ROWS
    FETCH NEXT @PageSize ROWS ONLY
END
GO
/****** Object:  StoredProcedure [dbo].[Event_ReadPageByOrganizationIdActingUserId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Event_ReadPageByOrganizationIdActingUserId]
    @OrganizationId UNIQUEIDENTIFIER,
    @ActingUserId UNIQUEIDENTIFIER,
    @StartDate DATETIME2(7),
    @EndDate DATETIME2(7),
    @BeforeDate DATETIME2(7),
    @PageSize INT
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[EventView]
    WHERE
        [Date] >= @StartDate
        AND (@BeforeDate IS NOT NULL OR [Date] <= @EndDate)
        AND (@BeforeDate IS NULL OR [Date] < @BeforeDate)
        AND [OrganizationId] = @OrganizationId
        AND [ActingUserId] = @ActingUserId
    ORDER BY [Date] DESC
    OFFSET 0 ROWS
    FETCH NEXT @PageSize ROWS ONLY
END
GO
/****** Object:  StoredProcedure [dbo].[Event_ReadPageByUserId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Event_ReadPageByUserId]
    @UserId UNIQUEIDENTIFIER,
    @StartDate DATETIME2(7),
    @EndDate DATETIME2(7),
    @BeforeDate DATETIME2(7),
    @PageSize INT
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[EventView]
    WHERE
        [Date] >= @StartDate
        AND (@BeforeDate IS NOT NULL OR [Date] <= @EndDate)
        AND (@BeforeDate IS NULL OR [Date] < @BeforeDate)
        AND [OrganizationId] IS NULL
        AND [ActingUserId] = @UserId
    ORDER BY [Date] DESC
    OFFSET 0 ROWS
    FETCH NEXT @PageSize ROWS ONLY
END
GO
/****** Object:  StoredProcedure [dbo].[Folder_Create]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Folder_Create]
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @Name VARCHAR(MAX),
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [dbo].[Folder]
    (
        [Id],
        [UserId],
        [Name],
        [CreationDate],
        [RevisionDate]
    )
    VALUES
    (
        @Id,
        @UserId,
        @Name,
        @CreationDate,
        @RevisionDate
    )

    EXEC [dbo].[User_BumpAccountRevisionDate] @UserId
END
GO
/****** Object:  StoredProcedure [dbo].[Folder_DeleteById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Folder_DeleteById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @UserId UNIQUEIDENTIFIER = (SELECT TOP 1 [UserId] FROM [dbo].[Folder] WHERE [Id] = @Id)
    DECLARE @UserIdPath VARCHAR(50) = CONCAT('$."', @UserId, '"')

    ;WITH [CTE] AS (
        SELECT
            [Id],
            [OrganizationId],
            [AccessAll]
        FROM
            [OrganizationUser]
        WHERE
            [UserId] = @UserId
            AND [Status] = 2 -- Confirmed
    )
    UPDATE
        C
    SET
        C.[Folders] = JSON_MODIFY(C.[Folders], @UserIdPath, NULL)
    FROM
        [dbo].[Cipher] C
    INNER JOIN
        [CTE] OU ON C.[UserId] IS NULL AND C.[OrganizationId] IN (SELECT [OrganizationId] FROM [CTE])
    INNER JOIN
        [dbo].[Organization] O ON O.[Id] = OU.[OrganizationId] AND O.[Id] = C.[OrganizationId] AND O.[Enabled] = 1
    LEFT JOIN
        [dbo].[CollectionCipher] CC ON OU.[AccessAll] = 0 AND CC.[CipherId] = C.[Id]
    LEFT JOIN
        [dbo].[CollectionUser] CU ON CU.[CollectionId] = CC.[CollectionId] AND CU.[OrganizationUserId] = OU.[Id]
    LEFT JOIN
        [dbo].[GroupUser] GU ON CU.[CollectionId] IS NULL AND OU.[AccessAll] = 0 AND GU.[OrganizationUserId] = OU.[Id]
    LEFT JOIN
        [dbo].[Group] G ON G.[Id] = GU.[GroupId]
    LEFT JOIN
        [dbo].[CollectionGroup] CG ON G.[AccessAll] = 0 AND CG.[CollectionId] = CC.[CollectionId] AND CG.[GroupId] = GU.[GroupId]
    WHERE
        (
            OU.[AccessAll] = 1
            OR CU.[CollectionId] IS NOT NULL
            OR G.[AccessAll] = 1
            OR CG.[CollectionId] IS NOT NULL
        )
        AND JSON_VALUE(C.[Folders], @UserIdPath) = @Id

    UPDATE
        C
    SET
        C.[Folders] = JSON_MODIFY(C.[Folders], @UserIdPath, NULL)
    FROM
        [dbo].[Cipher] C
    WHERE
        [UserId] = @UserId
        AND JSON_VALUE([Folders], @UserIdPath) = @Id

    DELETE
    FROM
        [dbo].[Folder]
    WHERE
        [Id] = @Id

    EXEC [dbo].[User_BumpAccountRevisionDate] @UserId
END
GO
/****** Object:  StoredProcedure [dbo].[Folder_ReadById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Folder_ReadById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[FolderView]
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[Folder_ReadByUserId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Folder_ReadByUserId]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[FolderView]
    WHERE
        [UserId] = @UserId
END
GO
/****** Object:  StoredProcedure [dbo].[Folder_Update]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Folder_Update]
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @Name VARCHAR(MAX),
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    UPDATE
        [dbo].[Folder]
    SET
        [UserId] = @UserId,
        [Name] = @Name,
        [CreationDate] = @CreationDate,
        [RevisionDate] = @RevisionDate
    WHERE
        [Id] = @Id

    EXEC [dbo].[User_BumpAccountRevisionDate] @UserId
END
GO
/****** Object:  StoredProcedure [dbo].[Grant_Delete]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Grant_Delete]
    @SubjectId NVARCHAR(200),
    @SessionId NVARCHAR(100),
    @ClientId NVARCHAR(200),
    @Type NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON

    DELETE
    FROM
        [dbo].[Grant]
    WHERE
        (@SubjectId IS NULL OR [SubjectId] = @SubjectId)
        AND (@ClientId IS NULL OR [ClientId] = @ClientId)
        AND (@SessionId IS NULL OR [SessionId] = @SessionId)
        AND (@Type IS NULL OR [Type] = @Type)
END
GO
/****** Object:  StoredProcedure [dbo].[Grant_DeleteByKey]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Grant_DeleteByKey]
    @Key NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON

    DELETE
    FROM
        [dbo].[Grant]
    WHERE
        [Key] = @Key
END
GO
/****** Object:  StoredProcedure [dbo].[Grant_DeleteExpired]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Grant_DeleteExpired]
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @BatchSize INT = 100
    DECLARE @Now DATETIME2(7) = GETUTCDATE()

    WHILE @BatchSize > 0
    BEGIN
        DELETE TOP(@BatchSize)
        FROM
            [dbo].[Grant]
        WHERE
            [ExpirationDate] < @Now

        SET @BatchSize = @@ROWCOUNT
    END
END
GO
/****** Object:  StoredProcedure [dbo].[Grant_Read]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Grant_Read]
    @SubjectId NVARCHAR(200),
    @SessionId NVARCHAR(100),
    @ClientId NVARCHAR(200),
    @Type NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[GrantView]
    WHERE
        (@SubjectId IS NULL OR [SubjectId] = @SubjectId)
        AND (@ClientId IS NULL OR [ClientId] = @ClientId)
        AND (@SessionId IS NULL OR [SessionId] = @SessionId)
        AND (@Type IS NULL OR [Type] = @Type)
END
GO
/****** Object:  StoredProcedure [dbo].[Grant_ReadByKey]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Grant_ReadByKey]
    @Key NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[GrantView]
    WHERE
        [Key] = @Key
END
GO
/****** Object:  StoredProcedure [dbo].[Grant_Save]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Grant_Save]
    @Key  NVARCHAR(200),
    @Type  NVARCHAR(50),
    @SubjectId NVARCHAR(200),
    @SessionId NVARCHAR(100),
    @ClientId NVARCHAR(200),
    @Description NVARCHAR(200),
    @CreationDate DATETIME2,
    @ExpirationDate DATETIME2,
    @ConsumedDate DATETIME2,
    @Data NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON

    MERGE
        [dbo].[Grant] AS [Target]
    USING
    ( 
        VALUES
        (
            @Key,
            @Type,
            @SubjectId,
            @SessionId,
            @ClientId,
            @Description,
            @CreationDate,
            @ExpirationDate,
            @ConsumedDate,
            @Data
        )
    ) AS [Source]
    (
        [Key],
        [Type],
        [SubjectId],
        [SessionId],
        [ClientId],
        [Description],
        [CreationDate],
        [ExpirationDate],
        [ConsumedDate],
        [Data]
    ) 
    ON
        [Target].[Key] = [Source].[Key]
    WHEN MATCHED THEN
        UPDATE
        SET
            [Type] = [Source].[Type],
            [SubjectId] = [Source].[SubjectId],
            [SessionId] = [Source].[SessionId],
            [ClientId] = [Source].[ClientId],
            [Description] = [Source].[Description],
            [CreationDate] = [Source].[CreationDate],
            [ExpirationDate] = [Source].[ExpirationDate],
            [ConsumedDate] = [Source].[ConsumedDate],
            [Data] = [Source].[Data]
    WHEN NOT MATCHED THEN
        INSERT
        (
            [Key],
            [Type],
            [SubjectId],
            [SessionId],
            [ClientId],
            [Description],
            [CreationDate],
            [ExpirationDate],
            [ConsumedDate],
            [Data]
        )
        VALUES
        (
            [Source].[Key],
            [Source].[Type],
            [Source].[SubjectId],
            [Source].[SessionId],
            [Source].[ClientId],
            [Source].[Description],
            [Source].[CreationDate],
            [Source].[ExpirationDate],
            [Source].[ConsumedDate],
            [Source].[Data]
        )
    ;
END
GO
/****** Object:  StoredProcedure [dbo].[Group_Create]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Group_Create]
    @Id UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @Name NVARCHAR(100),
    @AccessAll BIT,
    @ExternalId NVARCHAR(300),
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [dbo].[Group]
    (
        [Id],
        [OrganizationId],
        [Name],
        [AccessAll],
        [ExternalId],
        [CreationDate],
        [RevisionDate]
    )
    VALUES
    (
        @Id,
        @OrganizationId,
        @Name,
        @AccessAll,
        @ExternalId,
        @CreationDate,
        @RevisionDate
    )
END
GO
/****** Object:  StoredProcedure [dbo].[Group_CreateWithCollections]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Group_CreateWithCollections]
    @Id UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @Name NVARCHAR(100),
    @AccessAll BIT,
    @ExternalId NVARCHAR(300),
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7),
    @Collections AS [dbo].[SelectionReadOnlyArray] READONLY
AS
BEGIN
    SET NOCOUNT ON

    EXEC [dbo].[Group_Create] @Id, @OrganizationId, @Name, @AccessAll, @ExternalId, @CreationDate, @RevisionDate

    ;WITH [AvailableCollectionsCTE] AS(
        SELECT
            [Id]
        FROM
            [dbo].[Collection]
        WHERE
            [OrganizationId] = @OrganizationId
    )
    INSERT INTO [dbo].[CollectionGroup]
    (
        [CollectionId],
        [GroupId],
        [ReadOnly],
        [HidePasswords]
    )
    SELECT
        [Id],
        @Id,
        [ReadOnly],
        [HidePasswords]
    FROM
        @Collections
    WHERE
        [Id] IN (SELECT [Id] FROM [AvailableCollectionsCTE])

    EXEC [dbo].[User_BumpAccountRevisionDateByOrganizationId] @OrganizationId
END
GO
/****** Object:  StoredProcedure [dbo].[Group_DeleteById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Group_DeleteById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @OrganizationId UNIQUEIDENTIFIER = (SELECT TOP 1 [OrganizationId] FROM [dbo].[Group] WHERE [Id] = @Id)
    IF @OrganizationId IS NOT NULL
    BEGIN
        EXEC [dbo].[User_BumpAccountRevisionDateByOrganizationId] @OrganizationId
    END

    DELETE
    FROM
        [dbo].[Group]
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[Group_ReadById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Group_ReadById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[GroupView]
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[Group_ReadByOrganizationId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Group_ReadByOrganizationId]
    @OrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[GroupView]
    WHERE
        [OrganizationId] = @OrganizationId
END
GO
/****** Object:  StoredProcedure [dbo].[Group_ReadWithCollectionsById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Group_ReadWithCollectionsById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    EXEC [dbo].[Group_ReadById] @Id

    SELECT
        [CollectionId] [Id],
        [ReadOnly],
        [HidePasswords]
    FROM
        [dbo].[CollectionGroup]
    WHERE
        [GroupId] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[Group_Update]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Group_Update]
    @Id UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @Name NVARCHAR(100),
    @AccessAll BIT,
    @ExternalId NVARCHAR(300),
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    UPDATE
        [dbo].[Group]
    SET
        [OrganizationId] = @OrganizationId,
        [Name] = @Name,
        [AccessAll] = @AccessAll,
        [ExternalId] = @ExternalId,
        [CreationDate] = @CreationDate,
        [RevisionDate] = @RevisionDate
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[Group_UpdateWithCollections]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Group_UpdateWithCollections]
    @Id UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @Name NVARCHAR(100),
    @AccessAll BIT,
    @ExternalId NVARCHAR(300),
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7),
    @Collections AS [dbo].[SelectionReadOnlyArray] READONLY
AS
BEGIN
    SET NOCOUNT ON

    EXEC [dbo].[Group_Update] @Id, @OrganizationId, @Name, @AccessAll, @ExternalId, @CreationDate, @RevisionDate

    ;WITH [AvailableCollectionsCTE] AS(
        SELECT
            Id
        FROM
            [dbo].[Collection]
        WHERE
            OrganizationId = @OrganizationId
    )
    MERGE
        [dbo].[CollectionGroup] AS [Target]
    USING 
        @Collections AS [Source]
    ON
        [Target].[CollectionId] = [Source].[Id]
        AND [Target].[GroupId] = @Id
    WHEN NOT MATCHED BY TARGET
    AND [Source].[Id] IN (SELECT [Id] FROM [AvailableCollectionsCTE]) THEN
        INSERT VALUES
        (
            [Source].[Id],
            @Id,
            [Source].[ReadOnly],
            [Source].[HidePasswords]
        )
    WHEN MATCHED AND (
        [Target].[ReadOnly] != [Source].[ReadOnly]
        OR [Target].[HidePasswords] != [Source].[HidePasswords]
    ) THEN
        UPDATE SET [Target].[ReadOnly] = [Source].[ReadOnly],
                   [Target].[HidePasswords] = [Source].[HidePasswords]
    WHEN NOT MATCHED BY SOURCE
    AND [Target].[GroupId] = @Id THEN
        DELETE
    ;

    EXEC [dbo].[User_BumpAccountRevisionDateByOrganizationId] @OrganizationId
END
GO
/****** Object:  StoredProcedure [dbo].[GroupUser_Delete]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GroupUser_Delete]
    @GroupId UNIQUEIDENTIFIER,
    @OrganizationUserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    DELETE
    FROM
        [dbo].[GroupUser]
    WHERE
        [GroupId] = @GroupId
        AND [OrganizationUserId] = @OrganizationUserId

    EXEC [dbo].[User_BumpAccountRevisionDateByOrganizationUserId] @OrganizationUserId
END
GO
/****** Object:  StoredProcedure [dbo].[GroupUser_ReadByOrganizationId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GroupUser_ReadByOrganizationId]
    @OrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        GU.*
    FROM
        [dbo].[GroupUser] GU
    INNER JOIN
        [dbo].[Group] G ON G.[Id] = GU.[GroupId]
    WHERE
        G.[OrganizationId] = @OrganizationId
END
GO
/****** Object:  StoredProcedure [dbo].[GroupUser_ReadGroupIdsByOrganizationUserId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GroupUser_ReadGroupIdsByOrganizationUserId]
    @OrganizationUserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        [GroupId]
    FROM
        [dbo].[GroupUser]
    WHERE
        [OrganizationUserId] = @OrganizationUserId
END
GO
/****** Object:  StoredProcedure [dbo].[GroupUser_ReadOrganizationUserIdsByGroupId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GroupUser_ReadOrganizationUserIdsByGroupId]
    @GroupId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        [OrganizationUserId]
    FROM
        [dbo].[GroupUser]
    WHERE
        [GroupId] = @GroupId
END
GO
/****** Object:  StoredProcedure [dbo].[GroupUser_UpdateGroups]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GroupUser_UpdateGroups]
    @OrganizationUserId UNIQUEIDENTIFIER,
    @GroupIds AS [dbo].[GuidIdArray] READONLY
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @OrgId UNIQUEIDENTIFIER = (
        SELECT TOP 1
            [OrganizationId]
        FROM
            [dbo].[OrganizationUser]
        WHERE
            [Id] = @OrganizationUserId
    )

    -- Insert
    INSERT INTO
        [dbo].[GroupUser]
    SELECT
        [Source].[Id],
        @OrganizationUserId
    FROM
        @GroupIds [Source]
    INNER JOIN
        [dbo].[Group] G ON G.[Id] = [Source].[Id] AND G.[OrganizationId] = @OrgId
    WHERE
        NOT EXISTS (
            SELECT
                1
            FROM
                [dbo].[GroupUser]
            WHERE
                [OrganizationUserId] = @OrganizationUserId
                AND [GroupId] = [Source].[Id]
        )
    
    -- Delete
    DELETE
        GU
    FROM
        [dbo].[GroupUser] GU
    WHERE
        GU.[OrganizationUserId] = @OrganizationUserId
        AND NOT EXISTS (
            SELECT
                1
            FROM
                @GroupIds
            WHERE
                [Id] = GU.[GroupId]
        )

    EXEC [dbo].[User_BumpAccountRevisionDateByOrganizationUserId] @OrganizationUserId
END
GO
/****** Object:  StoredProcedure [dbo].[GroupUser_UpdateUsers]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[GroupUser_UpdateUsers]
    @GroupId UNIQUEIDENTIFIER,
    @OrganizationUserIds AS [dbo].[GuidIdArray] READONLY
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @OrgId UNIQUEIDENTIFIER = (
        SELECT TOP 1
            [OrganizationId]
        FROM
            [dbo].[Group]
        WHERE
            [Id] = @GroupId
    )

    -- Insert
    INSERT INTO
        [dbo].[GroupUser]
    SELECT
        @GroupId,
        [Source].[Id]
    FROM
        @OrganizationUserIds AS [Source]
    INNER JOIN
        [dbo].[OrganizationUser] OU ON [Source].[Id] = OU.[Id] AND OU.[OrganizationId] = @OrgId
    WHERE
        NOT EXISTS (
            SELECT
                1
            FROM
                [dbo].[GroupUser]
            WHERE
                [GroupId] = @GroupId
                AND [OrganizationUserId] = [Source].[Id]
        )
    
    -- Delete
    DELETE
        GU
    FROM
        [dbo].[GroupUser] GU
    WHERE
        GU.[GroupId] = @GroupId
        AND NOT EXISTS (
            SELECT
                1
            FROM
                @OrganizationUserIds
            WHERE
                [Id] = GU.[OrganizationUserId]
        )

    EXEC [dbo].[User_BumpAccountRevisionDateByOrganizationId] @OrgId
END
GO
/****** Object:  StoredProcedure [dbo].[Installation_Create]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Installation_Create]
    @Id UNIQUEIDENTIFIER,
    @Email NVARCHAR(50),
    @Key VARCHAR(150),
    @Enabled BIT,
    @CreationDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [dbo].[Installation]
    (
        [Id],
        [Email],
        [Key],
        [Enabled],
        [CreationDate]
    )
    VALUES
    (
        @Id,
        @Email,
        @Key,
        @Enabled,
        @CreationDate
    )
END
GO
/****** Object:  StoredProcedure [dbo].[Installation_DeleteById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Installation_DeleteById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    DELETE
    FROM
        [dbo].[Installation]
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[Installation_ReadById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Installation_ReadById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[InstallationView]
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[Installation_Update]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Installation_Update]
    @Id UNIQUEIDENTIFIER,
    @Email NVARCHAR(50),
    @Key VARCHAR(150),
    @Enabled BIT,
    @CreationDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    UPDATE
        [dbo].[Installation]
    SET
        [Email] = @Email,
        [Key] = @Key,
        [Enabled] = @Enabled,
        [CreationDate] = @CreationDate
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[Organization_Create]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Organization_Create]
    @Id UNIQUEIDENTIFIER,
    @Identifier NVARCHAR(50),
    @Name NVARCHAR(50),
    @BusinessName NVARCHAR(50),
    @BusinessAddress1 NVARCHAR(50),
    @BusinessAddress2 NVARCHAR(50),
    @BusinessAddress3 NVARCHAR(50),
    @BusinessCountry VARCHAR(2),
    @BusinessTaxNumber NVARCHAR(30),
    @BillingEmail NVARCHAR(50),
    @Plan NVARCHAR(50),
    @PlanType TINYINT,
    @Seats SMALLINT,
    @MaxCollections SMALLINT,
    @UsePolicies BIT,
    @UseSso BIT,
    @UseGroups BIT,
    @UseDirectory BIT,
    @UseEvents BIT,
    @UseTotp BIT,
    @Use2fa BIT,
    @UseApi BIT,
    @SelfHost BIT,
    @UsersGetPremium BIT,
    @Storage BIGINT,
    @MaxStorageGb SMALLINT,
    @Gateway TINYINT,
    @GatewayCustomerId VARCHAR(50),
    @GatewaySubscriptionId VARCHAR(50),
    @ReferenceData VARCHAR(MAX),
    @Enabled BIT,
    @LicenseKey VARCHAR(100),
    @ApiKey VARCHAR(30),
    @TwoFactorProviders NVARCHAR(MAX),
    @ExpirationDate DATETIME2(7),
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [dbo].[Organization]
    (
        [Id],
        [Identifier],
        [Name],
        [BusinessName],
        [BusinessAddress1],
        [BusinessAddress2],
        [BusinessAddress3],
        [BusinessCountry],
        [BusinessTaxNumber],
        [BillingEmail],
        [Plan],
        [PlanType],
        [Seats],
        [MaxCollections],
        [UsePolicies],
        [UseSso],
        [UseGroups],
        [UseDirectory],
        [UseEvents],
        [UseTotp],
        [Use2fa],
        [UseApi],
        [SelfHost],
        [UsersGetPremium],
        [Storage],
        [MaxStorageGb],
        [Gateway],
        [GatewayCustomerId],
        [GatewaySubscriptionId],
        [ReferenceData],
        [Enabled],
        [LicenseKey],
        [ApiKey],
        [TwoFactorProviders],
        [ExpirationDate],
        [CreationDate],
        [RevisionDate]
    )
    VALUES
    (
        @Id,
        @Identifier,
        @Name,
        @BusinessName,
        @BusinessAddress1,
        @BusinessAddress2,
        @BusinessAddress3,
        @BusinessCountry,
        @BusinessTaxNumber,
        @BillingEmail,
        @Plan,
        @PlanType,
        @Seats,
        @MaxCollections,
        @UsePolicies,
        @UseSso,
        @UseGroups,
        @UseDirectory,
        @UseEvents,
        @UseTotp,
        @Use2fa,
        @UseApi,
        @SelfHost,
        @UsersGetPremium,
        @Storage,
        @MaxStorageGb,
        @Gateway,
        @GatewayCustomerId,
        @GatewaySubscriptionId,
        @ReferenceData,
        @Enabled,
        @LicenseKey,
        @ApiKey,
        @TwoFactorProviders,
        @ExpirationDate,
        @CreationDate,
        @RevisionDate
    )
END
GO
/****** Object:  StoredProcedure [dbo].[Organization_DeleteById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Organization_DeleteById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    EXEC [dbo].[User_BumpAccountRevisionDateByOrganizationId] @Id

    DECLARE @BatchSize INT = 100
    WHILE @BatchSize > 0
    BEGIN
        BEGIN TRANSACTION Organization_DeleteById_Ciphers

        DELETE TOP(@BatchSize)
        FROM
            [dbo].[Cipher]
        WHERE
            [UserId] IS NULL
            AND [OrganizationId] = @Id

        SET @BatchSize = @@ROWCOUNT

        COMMIT TRANSACTION Organization_DeleteById_Ciphers
    END

    BEGIN TRANSACTION Organization_DeleteById

    DELETE
    FROM
        [dbo].[SsoUser]
    WHERE
        [OrganizationId] = @Id

    DELETE
    FROM
        [dbo].[SsoConfig]
    WHERE
        [OrganizationId] = @Id

    DELETE CU
    FROM 
        [dbo].[CollectionUser] CU
    INNER JOIN 
        [dbo].[OrganizationUser] OU ON [CU].[OrganizationUserId] = [OU].[Id]
    WHERE 
        [OU].[OrganizationId] = @Id

    DELETE
    FROM 
        [dbo].[OrganizationUser]
    WHERE 
        [OrganizationId] = @Id

    DELETE
    FROM
        [dbo].[Organization]
    WHERE
        [Id] = @Id

    COMMIT TRANSACTION Organization_DeleteById
END
GO
/****** Object:  StoredProcedure [dbo].[Organization_ReadAbilities]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Organization_ReadAbilities]
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        [Id],
        [UseEvents],
        [Use2fa],
        CASE 
        WHEN [Use2fa] = 1 AND [TwoFactorProviders] IS NOT NULL AND [TwoFactorProviders] != '{}' THEN
            1
        ELSE
            0
        END AS [Using2fa],
        [UsersGetPremium],
        [UseSso],
        [Enabled]
    FROM
        [dbo].[Organization]
END
GO
/****** Object:  StoredProcedure [dbo].[Organization_ReadByEnabled]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Organization_ReadByEnabled]
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[OrganizationView]
    WHERE
        [Enabled] = 1
END
GO
/****** Object:  StoredProcedure [dbo].[Organization_ReadById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Organization_ReadById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[OrganizationView]
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[Organization_ReadByIdentifier]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Organization_ReadByIdentifier]
    @Identifier NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[OrganizationView]
    WHERE
        [Identifier] = @Identifier
END
GO
/****** Object:  StoredProcedure [dbo].[Organization_ReadByUserId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Organization_ReadByUserId]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        O.*
    FROM
        [dbo].[OrganizationView] O
    INNER JOIN
        [dbo].[OrganizationUser] OU ON O.[Id] = OU.[OrganizationId]
    WHERE
        OU.[UserId] = @UserId
END
GO
/****** Object:  StoredProcedure [dbo].[Organization_Search]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Organization_Search]
    @Name NVARCHAR(50),
    @UserEmail NVARCHAR(50),
    @Paid BIT,
    @Skip INT = 0,
    @Take INT = 25
WITH RECOMPILE
AS
BEGIN
    SET NOCOUNT ON
    DECLARE @NameLikeSearch NVARCHAR(55) = '%' + @Name + '%'

    IF @UserEmail IS NOT NULL
    BEGIN
        SELECT
            O.*
        FROM
            [dbo].[OrganizationView] O
        INNER JOIN
            [dbo].[OrganizationUser] OU ON O.[Id] = OU.[OrganizationId]
        INNER JOIN
            [dbo].[User] U ON U.[Id] = OU.[UserId]
        WHERE
            (@Name IS NULL OR O.[Name] LIKE @NameLikeSearch)
            AND (@UserEmail IS NULL OR U.[Email] = @UserEmail)
            AND
            (
                @Paid IS NULL OR
                (
                    (@Paid = 1 AND O.[GatewaySubscriptionId] IS NOT NULL) OR
                    (@Paid = 0 AND O.[GatewaySubscriptionId] IS NULL)
                )
            )
        ORDER BY O.[CreationDate] DESC
        OFFSET @Skip ROWS
        FETCH NEXT @Take ROWS ONLY
    END
    ELSE
    BEGIN
        SELECT
            O.*
        FROM
            [dbo].[OrganizationView] O
        WHERE
            (@Name IS NULL OR O.[Name] LIKE @NameLikeSearch)
            AND
            (
                @Paid IS NULL OR
                (
                    (@Paid = 1 AND O.[GatewaySubscriptionId] IS NOT NULL) OR
                    (@Paid = 0 AND O.[GatewaySubscriptionId] IS NULL)
                )
            )
        ORDER BY O.[CreationDate] DESC
        OFFSET @Skip ROWS
        FETCH NEXT @Take ROWS ONLY
    END
END
GO
/****** Object:  StoredProcedure [dbo].[Organization_Update]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Organization_Update]
    @Id UNIQUEIDENTIFIER,
    @Identifier NVARCHAR(50),
    @Name NVARCHAR(50),
    @BusinessName NVARCHAR(50),
    @BusinessAddress1 NVARCHAR(50),
    @BusinessAddress2 NVARCHAR(50),
    @BusinessAddress3 NVARCHAR(50),
    @BusinessCountry VARCHAR(2),
    @BusinessTaxNumber NVARCHAR(30),
    @BillingEmail NVARCHAR(50),
    @Plan NVARCHAR(50),
    @PlanType TINYINT,
    @Seats SMALLINT,
    @MaxCollections SMALLINT,
    @UsePolicies BIT,
    @UseSso BIT,
    @UseGroups BIT,
    @UseDirectory BIT,
    @UseEvents BIT,
    @UseTotp BIT,
    @Use2fa BIT,
    @UseApi BIT,
    @SelfHost BIT,
    @UsersGetPremium BIT,
    @Storage BIGINT,
    @MaxStorageGb SMALLINT,
    @Gateway TINYINT,
    @GatewayCustomerId VARCHAR(50),
    @GatewaySubscriptionId VARCHAR(50),
    @ReferenceData VARCHAR(MAX),
    @Enabled BIT,
    @LicenseKey VARCHAR(100),
    @ApiKey VARCHAR(30),
    @TwoFactorProviders NVARCHAR(MAX),
    @ExpirationDate DATETIME2(7),
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    UPDATE
        [dbo].[Organization]
    SET
        [Identifier] = @Identifier,
        [Name] = @Name,
        [BusinessName] = @BusinessName,
        [BusinessAddress1] = @BusinessAddress1,
        [BusinessAddress2] = @BusinessAddress2,
        [BusinessAddress3] = @BusinessAddress3,
        [BusinessCountry] = @BusinessCountry,
        [BusinessTaxNumber] = @BusinessTaxNumber,
        [BillingEmail] = @BillingEmail,
        [Plan] = @Plan,
        [PlanType] = @PlanType,
        [Seats] = @Seats,
        [MaxCollections] = @MaxCollections,
        [UsePolicies] = @UsePolicies,
        [UseSso] = @UseSso,
        [UseGroups] = @UseGroups,
        [UseDirectory] = @UseDirectory,
        [UseEvents] = @UseEvents,
        [UseTotp] = @UseTotp,
        [Use2fa] = @Use2fa,
        [UseApi] = @UseApi,
        [SelfHost] = @SelfHost,
        [UsersGetPremium] = @UsersGetPremium,
        [Storage] = @Storage,
        [MaxStorageGb] = @MaxStorageGb,
        [Gateway] = @Gateway,
        [GatewayCustomerId] = @GatewayCustomerId,
        [GatewaySubscriptionId] = @GatewaySubscriptionId,
        [ReferenceData] = @ReferenceData,
        [Enabled] = @Enabled,
        [LicenseKey] = @LicenseKey,
        [ApiKey] = @ApiKey,
        [TwoFactorProviders] = @TwoFactorProviders,
        [ExpirationDate] = @ExpirationDate,
        [CreationDate] = @CreationDate,
        [RevisionDate] = @RevisionDate
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[Organization_UpdateStorage]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Organization_UpdateStorage]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @AttachmentStorage BIGINT
    DECLARE @SendStorage BIGINT

    CREATE TABLE #OrgStorageUpdateTemp
    ( 
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [Attachments] VARCHAR(MAX) NULL
    )

    INSERT INTO #OrgStorageUpdateTemp
    SELECT
        [Id],
        [Attachments]
    FROM
        [dbo].[Cipher]
    WHERE
        [UserId] IS NULL
        AND [OrganizationId] = @Id

    ;WITH [CTE] AS (
        SELECT
            [Id],
            (
                SELECT
                    SUM(CAST(JSON_VALUE(value,'$.Size') AS BIGINT))
                FROM
                    OPENJSON([Attachments])
            ) [Size]
        FROM
            #OrgStorageUpdateTemp
    )
    SELECT
        @AttachmentStorage = SUM([Size])
    FROM
        [CTE]

    DROP TABLE #OrgStorageUpdateTemp

    ;WITH [CTE] AS (
        SELECT
            [Id],
            CAST(JSON_VALUE([Data],'$.Size') AS BIGINT) [Size]
        FROM
            [Send]
        WHERE
            [UserId] IS NULL
            AND [OrganizationId] = @Id
    )
    SELECT
        @SendStorage = SUM([CTE].[Size])
    FROM
        [CTE]

    UPDATE
        [dbo].[Organization]
    SET
        [Storage] = (ISNULL(@AttachmentStorage, 0) + ISNULL(@SendStorage, 0)),
        [RevisionDate] = GETUTCDATE()
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[OrganizationUser_Create]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[OrganizationUser_Create]
    @Id UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @Email NVARCHAR(50),
    @Key VARCHAR(MAX),
    @Status TINYINT,
    @Type TINYINT,
    @AccessAll BIT,
    @ExternalId NVARCHAR(300),
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7),
    @Permissions NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [dbo].[OrganizationUser]
    (
        [Id],
        [OrganizationId],
        [UserId],
        [Email],
        [Key],
        [Status],
        [Type],
        [AccessAll],
        [ExternalId],
        [CreationDate],
        [RevisionDate],
        [Permissions]
    )
    VALUES
    (
        @Id,
        @OrganizationId,
        @UserId,
        @Email,
        @Key,
        @Status,
        @Type,
        @AccessAll,
        @ExternalId,
        @CreationDate,
        @RevisionDate,
        @Permissions
    )
END
GO
/****** Object:  StoredProcedure [dbo].[OrganizationUser_CreateWithCollections]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[OrganizationUser_CreateWithCollections]
    @Id UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @Email NVARCHAR(50),
    @Key VARCHAR(MAX),
    @Status TINYINT,
    @Type TINYINT,
    @AccessAll BIT,
    @ExternalId NVARCHAR(300),
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7),
    @Permissions NVARCHAR(MAX),
    @Collections AS [dbo].[SelectionReadOnlyArray] READONLY
AS
BEGIN
    SET NOCOUNT ON

    EXEC [dbo].[OrganizationUser_Create] @Id, @OrganizationId, @UserId, @Email, @Key, @Status, @Type, @AccessAll, @ExternalId, @CreationDate, @RevisionDate, @Permissions

    ;WITH [AvailableCollectionsCTE] AS(
        SELECT
            [Id]
        FROM
            [dbo].[Collection]
        WHERE
            [OrganizationId] = @OrganizationId
    )
    INSERT INTO [dbo].[CollectionUser]
    (
        [CollectionId],
        [OrganizationUserId],
        [ReadOnly],
        [HidePasswords]
    )
    SELECT
        [Id],
        @Id,
        [ReadOnly],
        [HidePasswords]
    FROM
        @Collections
    WHERE
        [Id] IN (SELECT [Id] FROM [AvailableCollectionsCTE])
END
GO
/****** Object:  StoredProcedure [dbo].[OrganizationUser_DeleteById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[OrganizationUser_DeleteById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON
    
    EXEC [dbo].[User_BumpAccountRevisionDateByOrganizationUserId] @Id
    
    DECLARE @OrganizationId UNIQUEIDENTIFIER
    DECLARE @UserId UNIQUEIDENTIFIER

    SELECT
        @OrganizationId = [OrganizationId],
        @UserId = [UserId]
    FROM
        [dbo].[OrganizationUser]
    WHERE
        [Id] = @Id

    IF @OrganizationId IS NOT NULL AND @UserId IS NOT NULL
    BEGIN
        EXEC [dbo].[SsoUser_Delete] @UserId, @OrganizationId
    END

    DELETE
    FROM
        [dbo].[CollectionUser]
    WHERE
        [OrganizationUserId] = @Id

    DELETE
    FROM
        [dbo].[GroupUser]
    WHERE
        [OrganizationUserId] = @Id

    DELETE
    FROM
        [dbo].[OrganizationUser]
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[OrganizationUser_ReadById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[OrganizationUser_ReadById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[OrganizationUserView]
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[OrganizationUser_ReadByOrganizationId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[OrganizationUser_ReadByOrganizationId]
    @OrganizationId UNIQUEIDENTIFIER,
    @Type TINYINT
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[OrganizationUserView]
    WHERE
        [OrganizationId] = @OrganizationId
        AND (@Type IS NULL OR [Type] = @Type)
END
GO
/****** Object:  StoredProcedure [dbo].[OrganizationUser_ReadByOrganizationIdEmail]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[OrganizationUser_ReadByOrganizationIdEmail]
    @OrganizationId UNIQUEIDENTIFIER,
    @Email NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[OrganizationUserView]
    WHERE
        [OrganizationId] = @OrganizationId
        AND [Email] IS NOT NULL
        AND @Email IS NOT NULL
        AND [Email] = @Email
END
GO
/****** Object:  StoredProcedure [dbo].[OrganizationUser_ReadByOrganizationIdUserId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[OrganizationUser_ReadByOrganizationIdUserId]
    @OrganizationId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[OrganizationUserView]
    WHERE
        [OrganizationId] = @OrganizationId
        AND [UserId] = @UserId
END
GO
/****** Object:  StoredProcedure [dbo].[OrganizationUser_ReadByUserId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[OrganizationUser_ReadByUserId]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[OrganizationUserView]
    WHERE
        [UserId] = @UserId
END
GO
/****** Object:  StoredProcedure [dbo].[OrganizationUser_ReadByUserIds]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[OrganizationUser_ReadByUserIds]
    @UserIds AS [dbo].[GuidIdArray] READONLY
AS
BEGIN
    SET NOCOUNT ON

    IF (SELECT COUNT(1) FROM @UserIds) < 1
    BEGIN
        RETURN(-1)
    END

    SELECT
        *
    FROM
        [dbo].[OrganizationUserView]
    WHERE
        [UserId] IN (SELECT [Id] FROM @UserIds)
END
GO
/****** Object:  StoredProcedure [dbo].[OrganizationUser_ReadCountByFreeOrganizationAdminUser]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[OrganizationUser_ReadCountByFreeOrganizationAdminUser]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        COUNT(1)
    FROM
        [dbo].[OrganizationUser] OU
    INNER JOIN
        [dbo].[Organization] O ON O.Id = OU.[OrganizationId]
    WHERE
        OU.[UserId] = @UserId
        AND OU.[Type] < 2 -- Owner or Admin
        AND O.[PlanType] = 0 -- Free
        AND OU.[Status] = 2 -- 2 = Confirmed
END
GO
/****** Object:  StoredProcedure [dbo].[OrganizationUser_ReadCountByOnlyOwner]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[OrganizationUser_ReadCountByOnlyOwner]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    ;WITH [OwnerCountCTE] AS
    (
        SELECT
            OU.[UserId],
            COUNT(1) OVER (PARTITION BY OU.[OrganizationId]) [ConfirmedOwnerCount]
        FROM
            [dbo].[OrganizationUser] OU
        WHERE
            OU.[Type] = 0 -- 0 = Owner
            AND OU.[Status] = 2 -- 2 = Confirmed
    )
    SELECT
        COUNT(1)
    FROM
        [OwnerCountCTE] OC
    WHERE
        OC.[UserId] = @UserId
        AND OC.[ConfirmedOwnerCount] = 1
END
GO
/****** Object:  StoredProcedure [dbo].[OrganizationUser_ReadCountByOrganizationId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[OrganizationUser_ReadCountByOrganizationId]
    @OrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        COUNT(1)
    FROM
        [dbo].[OrganizationUser]
    WHERE
        [OrganizationId] = @OrganizationId
END
GO
/****** Object:  StoredProcedure [dbo].[OrganizationUser_ReadCountByOrganizationIdEmail]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[OrganizationUser_ReadCountByOrganizationIdEmail]
    @OrganizationId UNIQUEIDENTIFIER,
    @Email NVARCHAR(50),
    @OnlyUsers BIT
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        COUNT(1)
    FROM
        [dbo].[OrganizationUser] OU
    LEFT JOIN
        [dbo].[User] U ON OU.[UserId] = U.[Id]
    WHERE
        OU.[OrganizationId] = @OrganizationId
        AND (
            (@OnlyUsers = 0 AND (OU.[Email] = @Email OR U.[Email] = @Email))
            OR (@OnlyUsers = 1 AND U.[Email] = @Email)
        )
END
GO
/****** Object:  StoredProcedure [dbo].[OrganizationUser_ReadWithCollectionsById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[OrganizationUser_ReadWithCollectionsById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    EXEC [OrganizationUser_ReadById] @Id

    SELECT
        CU.[CollectionId] Id,
        CU.[ReadOnly],
        CU.[HidePasswords]
    FROM
        [dbo].[OrganizationUser] OU
    INNER JOIN
        [dbo].[CollectionUser] CU ON OU.[AccessAll] = 0 AND CU.[OrganizationUserId] = [OU].[Id]
    WHERE
        [OrganizationUserId] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[OrganizationUser_Update]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[OrganizationUser_Update]
    @Id UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @Email NVARCHAR(50),
    @Key VARCHAR(MAX),
    @Status TINYINT,
    @Type TINYINT,
    @AccessAll BIT,
    @ExternalId NVARCHAR(300),
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7),
    @Permissions NVARCHAR(MAX)
AS
BEGIN
    SET NOCOUNT ON

    UPDATE
        [dbo].[OrganizationUser]
    SET
        [OrganizationId] = @OrganizationId,
        [UserId] = @UserId,
        [Email] = @Email,
        [Key] = @Key,
        [Status] = @Status,
        [Type] = @Type,
        [AccessAll] = @AccessAll,
        [ExternalId] = @ExternalId,
        [CreationDate] = @CreationDate,
        [RevisionDate] = @RevisionDate,
        [Permissions] = @Permissions
    WHERE
        [Id] = @Id

    EXEC [dbo].[User_BumpAccountRevisionDate] @UserId
END
GO
/****** Object:  StoredProcedure [dbo].[OrganizationUser_UpdateWithCollections]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[OrganizationUser_UpdateWithCollections]
    @Id UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @Email NVARCHAR(50),
    @Key VARCHAR(MAX),
    @Status TINYINT,
    @Type TINYINT,
    @AccessAll BIT,
    @ExternalId NVARCHAR(300),
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7),
    @Permissions NVARCHAR(MAX),
    @Collections AS [dbo].[SelectionReadOnlyArray] READONLY
AS
BEGIN
    SET NOCOUNT ON

    EXEC [dbo].[OrganizationUser_Update] @Id, @OrganizationId, @UserId, @Email, @Key, @Status, @Type, @AccessAll, @ExternalId, @CreationDate, @RevisionDate, @Permissions


    -- Update
    UPDATE
        [Target]
    SET
        [Target].[ReadOnly] = [Source].[ReadOnly],
        [Target].[HidePasswords] = [Source].[HidePasswords]
    FROM
        [dbo].[CollectionUser] AS [Target]
    INNER JOIN
        @Collections AS [Source] ON [Source].[Id] = [Target].[CollectionId]
    WHERE
        [Target].[OrganizationUserId] = @Id
        AND (
            [Target].[ReadOnly] != [Source].[ReadOnly]
            OR [Target].[HidePasswords] != [Source].[HidePasswords]
        )

    -- Insert
    INSERT INTO
        [dbo].[CollectionUser]
    SELECT
        [Source].[Id],
        @Id,
        [Source].[ReadOnly],
        [Source].[HidePasswords]
    FROM
        @Collections AS [Source]
    INNER JOIN
        [dbo].[Collection] C ON C.[Id] = [Source].[Id] AND C.[OrganizationId] = @OrganizationId
    WHERE
        NOT EXISTS (
            SELECT
                1
            FROM
                [dbo].[CollectionUser]
            WHERE
                [CollectionId] = [Source].[Id]
                AND [OrganizationUserId] = @Id
        )
    
    -- Delete
    DELETE
        CU
    FROM
        [dbo].[CollectionUser] CU
    WHERE
        CU.[OrganizationUserId] = @Id
        AND NOT EXISTS (
            SELECT
                1
            FROM
                @Collections
            WHERE
                [Id] = CU.[CollectionId]
        )
END
GO
/****** Object:  StoredProcedure [dbo].[OrganizationUserOrganizationDetails_ReadByUserIdStatus]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[OrganizationUserOrganizationDetails_ReadByUserIdStatus]
    @UserId UNIQUEIDENTIFIER,
    @Status TINYINT
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[OrganizationUserOrganizationDetailsView]
    WHERE
        [UserId] = @UserId
        AND (@Status IS NULL OR [Status] = @Status)
END
GO
/****** Object:  StoredProcedure [dbo].[OrganizationUserOrganizationDetails_ReadByUserIdStatusOrganizationId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[OrganizationUserOrganizationDetails_ReadByUserIdStatusOrganizationId]
    @UserId UNIQUEIDENTIFIER,
    @Status TINYINT,
    @OrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[OrganizationUserOrganizationDetailsView]
    WHERE
        [UserId] = @UserId
        AND [OrganizationId] = @OrganizationId
        AND (@Status IS NULL OR [Status] = @Status)
END
GO
/****** Object:  StoredProcedure [dbo].[OrganizationUserUserDetails_ReadById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[OrganizationUserUserDetails_ReadById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[OrganizationUserUserDetailsView]
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[OrganizationUserUserDetails_ReadByOrganizationId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[OrganizationUserUserDetails_ReadByOrganizationId]
    @OrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[OrganizationUserUserDetailsView]
    WHERE
        [OrganizationId] = @OrganizationId
END
GO
/****** Object:  StoredProcedure [dbo].[OrganizationUserUserDetails_ReadWithCollectionsById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[OrganizationUserUserDetails_ReadWithCollectionsById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    EXEC [OrganizationUserUserDetails_ReadById] @Id

    SELECT
        CU.[CollectionId] Id,
        CU.[ReadOnly],
        CU.[HidePasswords]
    FROM
        [dbo].[OrganizationUser] OU
    INNER JOIN
        [dbo].[CollectionUser] CU ON OU.[AccessAll] = 0 AND CU.[OrganizationUserId] = [OU].[Id]
    WHERE
        [OrganizationUserId] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[Policy_Create]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Policy_Create]
    @Id UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @Type TINYINT,
    @Data NVARCHAR(MAX),
    @Enabled BIT,
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [dbo].[Policy]
    (
        [Id],
        [OrganizationId],
        [Type],
        [Data],
        [Enabled],
        [CreationDate],
        [RevisionDate]
    )
    VALUES
    (
        @Id,
        @OrganizationId,
        @Type,
        @Data,
        @Enabled,
        @CreationDate,
        @RevisionDate
    )

    EXEC [dbo].[User_BumpAccountRevisionDateByOrganizationId] @OrganizationId
END
GO
/****** Object:  StoredProcedure [dbo].[Policy_DeleteById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Policy_DeleteById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @OrganizationId UNIQUEIDENTIFIER = (SELECT TOP 1 [OrganizationId] FROM [dbo].[Policy] WHERE [Id] = @Id)
    IF @OrganizationId IS NOT NULL
    BEGIN
        EXEC [dbo].[User_BumpAccountRevisionDateByOrganizationId] @OrganizationId
    END

    DELETE
    FROM
        [dbo].[Policy]
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[Policy_ReadById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Policy_ReadById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[PolicyView]
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[Policy_ReadByOrganizationId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Policy_ReadByOrganizationId]
    @OrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[PolicyView]
    WHERE
        [OrganizationId] = @OrganizationId
END
GO
/****** Object:  StoredProcedure [dbo].[Policy_ReadByOrganizationIdType]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Policy_ReadByOrganizationIdType]
    @OrganizationId UNIQUEIDENTIFIER,
    @Type TINYINT
AS
BEGIN
    SET NOCOUNT ON

    SELECT TOP 1
        *
    FROM
        [dbo].[PolicyView]
    WHERE
        [OrganizationId] = @OrganizationId
        AND [Type] = @Type
END
GO
/****** Object:  StoredProcedure [dbo].[Policy_ReadByUserId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Policy_ReadByUserId]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        P.*
    FROM
        [dbo].[PolicyView] P
    INNER JOIN
        [dbo].[OrganizationUser] OU ON P.[OrganizationId] = OU.[OrganizationId]
    INNER JOIN
        [dbo].[Organization] O ON OU.[OrganizationId] = O.[Id]
    WHERE
        OU.[UserId] = @UserId
        AND OU.[Status] = 2 -- 2 = Confirmed
        AND O.[Enabled] = 1
END
GO
/****** Object:  StoredProcedure [dbo].[Policy_Update]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Policy_Update]
    @Id UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @Type TINYINT,
    @Data NVARCHAR(MAX),
    @Enabled BIT,
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    UPDATE
        [dbo].[Policy]
    SET
        [OrganizationId] = @OrganizationId,
        [Type] = @Type,
        [Data] = @Data,
        [Enabled] = @Enabled,
        [CreationDate] = @CreationDate,
        [RevisionDate] = @RevisionDate
    WHERE
        [Id] = @Id

    EXEC [dbo].[User_BumpAccountRevisionDateByOrganizationId] @OrganizationId
END
GO
/****** Object:  StoredProcedure [dbo].[Send_Create]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Send_Create]
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @Type TINYINT,
    @Data VARCHAR(MAX),
    @Key VARCHAR(MAX),
    @Password NVARCHAR(300),
    @MaxAccessCount INT,
    @AccessCount INT,
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7),
    @ExpirationDate DATETIME2(7),
    @DeletionDate DATETIME2(7),
    @Disabled BIT
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [dbo].[Send]
    (
        [Id],
        [UserId],
        [OrganizationId],
        [Type],
        [Data],
        [Key],
        [Password],
        [MaxAccessCount],
        [AccessCount],
        [CreationDate],
        [RevisionDate],
        [ExpirationDate],
        [DeletionDate],
        [Disabled]
    )
    VALUES
    (
        @Id,
        @UserId,
        @OrganizationId,
        @Type,
        @Data,
        @Key,
        @Password,
        @MaxAccessCount,
        @AccessCount,
        @CreationDate,
        @RevisionDate,
        @ExpirationDate,
        @DeletionDate,
        @Disabled
    )

    IF @UserId IS NOT NULL
    BEGIN
        IF @Type = 1 --File
        BEGIN
            EXEC [dbo].[User_UpdateStorage] @UserId
        END
        EXEC [dbo].[User_BumpAccountRevisionDate] @UserId
    END
    -- TODO: OrganizationId bump?
END
GO
/****** Object:  StoredProcedure [dbo].[Send_DeleteById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Send_DeleteById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @UserId UNIQUEIDENTIFIER
    DECLARE @OrganizationId UNIQUEIDENTIFIER
    DECLARE @Type TINYINT

    SELECT TOP 1
        @UserId = [UserId],
        @OrganizationId = [OrganizationId],
        @Type = [Type]
    FROM
        [dbo].[Send]
    WHERE
        [Id] = @Id

    DELETE
    FROM
        [dbo].[Send]
    WHERE
        [Id] = @Id

    IF @UserId IS NOT NULL
    BEGIN
        IF @Type = 1 --File
        BEGIN
            EXEC [dbo].[User_UpdateStorage] @UserId
        END
        EXEC [dbo].[User_BumpAccountRevisionDate] @UserId
    END
    -- TODO: OrganizationId bump?
END
GO
/****** Object:  StoredProcedure [dbo].[Send_ReadByDeletionDateBefore]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Send_ReadByDeletionDateBefore]
    @DeletionDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[SendView]
    WHERE
        [DeletionDate] < @DeletionDate
END
GO
/****** Object:  StoredProcedure [dbo].[Send_ReadById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Send_ReadById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[SendView]
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[Send_ReadByUserId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Send_ReadByUserId]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[SendView]
    WHERE
        [OrganizationId] IS NULL
        AND [UserId] = @UserId
END
GO
/****** Object:  StoredProcedure [dbo].[Send_Update]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Send_Update]
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @Type TINYINT,
    @Data VARCHAR(MAX),
    @Key VARCHAR(MAX),
    @Password NVARCHAR(300),
    @MaxAccessCount INT,
    @AccessCount INT,
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7),
    @ExpirationDate DATETIME2(7),
    @DeletionDate DATETIME2(7),
    @Disabled BIT
AS
BEGIN
    SET NOCOUNT ON

    UPDATE
        [dbo].[Send]
    SET
        [UserId] = @UserId,
        [OrganizationId] = @OrganizationId,
        [Type] = @Type,
        [Data] = @Data,
        [Key] = @Key,
        [Password] = @Password,
        [MaxAccessCount] = @MaxAccessCount,
        [AccessCount] = @AccessCount,
        [CreationDate] = @CreationDate,
        [RevisionDate] = @RevisionDate,
        [ExpirationDate] = @ExpirationDate,
        [DeletionDate] = @DeletionDate,
        [Disabled] = @Disabled
    WHERE
        [Id] = @Id

    IF @UserId IS NOT NULL
    BEGIN
        EXEC [dbo].[User_BumpAccountRevisionDate] @UserId
    END
    -- TODO: OrganizationId bump?
END
GO
/****** Object:  StoredProcedure [dbo].[SsoConfig_Create]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SsoConfig_Create]
    @Id BIGINT OUTPUT,
    @Enabled BIT,
    @OrganizationId UNIQUEIDENTIFIER,
    @Data NVARCHAR(MAX),
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [dbo].[SsoConfig]
    (
        [Enabled],
        [OrganizationId],
        [Data],
        [CreationDate],
        [RevisionDate]
    )
    VALUES
    (
        @Enabled,
        @OrganizationId,
        @Data,
        @CreationDate,
        @RevisionDate
    )

    SET @Id = SCOPE_IDENTITY();
END
GO
/****** Object:  StoredProcedure [dbo].[SsoConfig_ReadByIdentifier]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SsoConfig_ReadByIdentifier]
    @Identifier NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON

    SELECT TOP 1
        SSO.*
    FROM
        [dbo].[SsoConfigView] SSO
    INNER JOIN
        [dbo].[Organization] O ON O.[Id] = SSO.[OrganizationId]
        AND O.[Identifier] = @Identifier
END
GO
/****** Object:  StoredProcedure [dbo].[SsoConfig_ReadByOrganizationId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SsoConfig_ReadByOrganizationId]
    @OrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT TOP 1
        *
    FROM
        [dbo].[SsoConfigView]
    WHERE
        [OrganizationId] = @OrganizationId
END
GO
/****** Object:  StoredProcedure [dbo].[SsoConfig_ReadManyByNotBeforeRevisionDate]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SsoConfig_ReadManyByNotBeforeRevisionDate]
    @NotBefore DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[SsoConfigView]
    WHERE
        [Enabled] = 1
        AND [RevisionDate] >= COALESCE(@NotBefore, [RevisionDate]);
END
GO
/****** Object:  StoredProcedure [dbo].[SsoConfig_Update]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SsoConfig_Update]
    @Id BIGINT,
    @Enabled BIT,
    @OrganizationId UNIQUEIDENTIFIER,
    @Data NVARCHAR(MAX),
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    UPDATE
        [dbo].[SsoConfig]
    SET
        [Enabled] = @Enabled,
        [OrganizationId] = @OrganizationId,
        [Data] = @Data,
        [CreationDate] = @CreationDate,
        [RevisionDate] = @RevisionDate
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[SsoUser_Create]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SsoUser_Create]
    @Id BIGINT OUTPUT,
    @UserId UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @ExternalId NVARCHAR(50),
    @CreationDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [dbo].[SsoUser]
    (
        [UserId],
        [OrganizationId],
        [ExternalId],
        [CreationDate]
    )
    VALUES
    (
        @UserId,
        @OrganizationId,
        @ExternalId,
        @CreationDate
    )

    SET @Id = SCOPE_IDENTITY();
END
GO
/****** Object:  StoredProcedure [dbo].[SsoUser_Delete]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SsoUser_Delete]
    @UserId UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    DELETE
    FROM
        [dbo].[SsoUser]
    WHERE
        [UserId] = @UserId
        AND [OrganizationId] = @OrganizationId
END
GO
/****** Object:  StoredProcedure [dbo].[SsoUser_ReadById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SsoUser_ReadById]
    @Id BIGINT
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[SsoUserView]
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[SsoUser_Update]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[SsoUser_Update]
    @Id BIGINT OUTPUT,
    @UserId UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @ExternalId NVARCHAR(50),
    @CreationDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    UPDATE
        [dbo].[SsoUser]
    SET
        [UserId] = @UserId,
        [OrganizationId] = @OrganizationId,
        [ExternalId] = @ExternalId,
        [CreationDate] = @CreationDate
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[TaxRate_Archive]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[TaxRate_Archive]
    @Id VARCHAR(40)
AS
BEGIN
    SET NOCOUNT ON

    UPDATE
        [dbo].[TaxRate]
    SET
        [Active] = 0
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[TaxRate_Create]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[TaxRate_Create]
    @Id VARCHAR(40),
    @Country VARCHAR(50),
    @State VARCHAR(2),
    @PostalCode VARCHAR(10),
    @Rate DECIMAL(5,2),
    @Active BIT
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [dbo].[TaxRate]
    (
        [Id],
        [Country],
        [State],
        [PostalCode],
        [Rate],
        [Active]
    )
    VALUES
    (
        @Id,
        @Country,
        @State,
        @PostalCode,
        @Rate,
        1
    )
END
GO
/****** Object:  StoredProcedure [dbo].[TaxRate_ReadAllActive]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[TaxRate_ReadAllActive]
AS
BEGIN
    SET NOCOUNT ON 
    
    SELECT * FROM [dbo].[TaxRate]
    WHERE Active = 1
END
GO
/****** Object:  StoredProcedure [dbo].[TaxRate_ReadById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[TaxRate_ReadById]
    @Id VARCHAR(40)
AS
BEGIN
    SET NOCOUNT ON 

    SELECT * FROM [dbo].[TaxRate]
    WHERE Id = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[TaxRate_ReadByLocation]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[TaxRate_ReadByLocation]
    @Country VARCHAR(50),
    @PostalCode VARCHAR(10)
AS
BEGIN
    SET NOCOUNT ON 
    
    SELECT * FROM [dbo].[TaxRate]
    WHERE Active = 1
        AND [Country] = @Country
        AND [PostalCode] = @PostalCode
END
GO
/****** Object:  StoredProcedure [dbo].[TaxRate_Search]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[TaxRate_Search]
    @Skip INT = 0,
    @Count INT = 25
AS
BEGIN
    SET NOCOUNT ON 
    
    SELECT * FROM [dbo].[TaxRate]
    WHERE Active = 1
    ORDER BY Country, PostalCode DESC
    OFFSET @Skip ROWS
    FETCH NEXT @Count ROWS ONLY
END
GO
/****** Object:  StoredProcedure [dbo].[Transaction_Create]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Transaction_Create]
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @Type TINYINT,
    @Amount MONEY,
    @Refunded BIT,
    @RefundedAmount MONEY,
    @Details NVARCHAR(100),
    @PaymentMethodType TINYINT,
    @Gateway TINYINT,
    @GatewayId VARCHAR(50),
    @CreationDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [dbo].[Transaction]
    (
        [Id],
        [UserId],
        [OrganizationId],
        [Type],
        [Amount],
        [Refunded],
        [RefundedAmount],
        [Details],
        [PaymentMethodType],
        [Gateway],
        [GatewayId],
        [CreationDate]
    )
    VALUES
    (
        @Id,
        @UserId,
        @OrganizationId,
        @Type,
        @Amount,
        @Refunded,
        @RefundedAmount,
        @Details,
        @PaymentMethodType,
        @Gateway,
        @GatewayId,
        @CreationDate
    )
END
GO
/****** Object:  StoredProcedure [dbo].[Transaction_DeleteById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Transaction_DeleteById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    DELETE
    FROM
        [dbo].[Transaction]
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[Transaction_ReadByGatewayId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Transaction_ReadByGatewayId]
    @Gateway TINYINT,
    @GatewayId VARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[TransactionView]
    WHERE
        [Gateway] = @Gateway
        AND [GatewayId] = @GatewayId
END
GO
/****** Object:  StoredProcedure [dbo].[Transaction_ReadById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Transaction_ReadById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[TransactionView]
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[Transaction_ReadByOrganizationId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Transaction_ReadByOrganizationId]
    @OrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[TransactionView]
    WHERE
        [UserId] IS NULL
        AND [OrganizationId] = @OrganizationId
END
GO
/****** Object:  StoredProcedure [dbo].[Transaction_ReadByUserId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Transaction_ReadByUserId]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[TransactionView]
    WHERE
        [UserId] = @UserId
END
GO
/****** Object:  StoredProcedure [dbo].[Transaction_Update]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[Transaction_Update]
    @Id UNIQUEIDENTIFIER,
    @UserId UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER,
    @Type TINYINT,
    @Amount MONEY,
    @Refunded BIT,
    @RefundedAmount MONEY,
    @Details NVARCHAR(100),
    @PaymentMethodType TINYINT,
    @Gateway TINYINT,
    @GatewayId VARCHAR(50),
    @CreationDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    UPDATE
        [dbo].[Transaction]
    SET
        [UserId] = @UserId,
        [OrganizationId] = @OrganizationId,
        [Type] = @Type,
        [Amount] = @Amount,
        [Refunded] = @Refunded,
        [RefundedAmount] = @RefundedAmount,
        [Details] = @Details,
        [PaymentMethodType] = @PaymentMethodType,
        [Gateway] = @Gateway,
        [GatewayId] = @GatewayId,
        [CreationDate] = @CreationDate
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[U2f_Create]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[U2f_Create]
    @Id INT,
    @UserId UNIQUEIDENTIFIER,
    @KeyHandle VARCHAR(200),
    @Challenge VARCHAR(200),
    @AppId VARCHAR(50),
    @Version VARCHAR(20),
    @CreationDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [dbo].[U2f]
    (
        [UserId],
        [KeyHandle],
        [Challenge],
        [AppId],
        [Version],
        [CreationDate]
    )
    VALUES
    (
        @UserId,
        @KeyHandle,
        @Challenge,
        @AppId,
        @Version,
        @CreationDate
    )
END
GO
/****** Object:  StoredProcedure [dbo].[U2f_DeleteByUserId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[U2f_DeleteByUserId]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    DELETE
    FROM
        [dbo].[U2f]
    WHERE
        [UserId] = @UserId
END
GO
/****** Object:  StoredProcedure [dbo].[U2f_DeleteOld]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[U2f_DeleteOld]
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @BatchSize INT = 100
    DECLARE @Threshold DATETIME2(7) = DATEADD (day, -7, GETUTCDATE())

    WHILE @BatchSize > 0
    BEGIN
        DELETE TOP(@BatchSize)
        FROM
            [dbo].[U2f]
        WHERE
            [CreationDate] IS NULL
            OR [CreationDate] < @Threshold

        SET @BatchSize = @@ROWCOUNT
    END
END
GO
/****** Object:  StoredProcedure [dbo].[U2f_ReadByUserId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[U2f_ReadByUserId]
    @UserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[U2fView]
    WHERE
        [UserId] = @UserId
END
GO
/****** Object:  StoredProcedure [dbo].[User_BumpAccountRevisionDate]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_BumpAccountRevisionDate]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    UPDATE
        [User]
    SET
        [AccountRevisionDate] = GETUTCDATE()
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[User_BumpAccountRevisionDateByCipherId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_BumpAccountRevisionDateByCipherId]
    @CipherId UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    UPDATE
        U
    SET
        U.[AccountRevisionDate] = GETUTCDATE()
    FROM
        [dbo].[User] U
    INNER JOIN
        [dbo].[OrganizationUser] OU ON OU.[UserId] = U.[Id]
    LEFT JOIN
        [dbo].[CollectionCipher] CC ON CC.[CipherId] = @CipherId
    LEFT JOIN
        [dbo].[CollectionUser] CU ON OU.[AccessAll] = 0 AND CU.[OrganizationUserId] = OU.[Id] AND CU.[CollectionId] = CC.[CollectionId]
    LEFT JOIN
        [dbo].[GroupUser] GU ON CU.[CollectionId] IS NULL AND OU.[AccessAll] = 0 AND GU.[OrganizationUserId] = OU.[Id]
    LEFT JOIN
        [dbo].[Group] G ON G.[Id] = GU.[GroupId]
    LEFT JOIN
        [dbo].[CollectionGroup] CG ON G.[AccessAll] = 0 AND CG.[GroupId] = GU.[GroupId] AND CG.[CollectionId] = CC.[CollectionId]
    WHERE
        OU.[OrganizationId] = @OrganizationId
        AND OU.[Status] = 2 -- 2 = Confirmed
        AND (
            CU.[CollectionId] IS NOT NULL
            OR CG.[CollectionId] IS NOT NULL
            OR OU.[AccessAll] = 1
            OR G.[AccessAll] = 1
        )
END
GO
/****** Object:  StoredProcedure [dbo].[User_BumpAccountRevisionDateByCollectionId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_BumpAccountRevisionDateByCollectionId]
    @CollectionId UNIQUEIDENTIFIER,
    @OrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    UPDATE
        U
    SET
        U.[AccountRevisionDate] = GETUTCDATE()
    FROM
        [dbo].[User] U
    INNER JOIN
        [dbo].[OrganizationUser] OU ON OU.[UserId] = U.[Id]
    LEFT JOIN
        [dbo].[CollectionUser] CU ON OU.[AccessAll] = 0 AND CU.[OrganizationUserId] = OU.[Id] AND CU.[CollectionId] = @CollectionId
    LEFT JOIN
        [dbo].[GroupUser] GU ON CU.[CollectionId] IS NULL AND OU.[AccessAll] = 0 AND GU.[OrganizationUserId] = OU.[Id]
    LEFT JOIN
        [dbo].[Group] G ON G.[Id] = GU.[GroupId]
    LEFT JOIN
        [dbo].[CollectionGroup] CG ON G.[AccessAll] = 0 AND CG.[GroupId] = GU.[GroupId] AND CG.[CollectionId] = @CollectionId
    WHERE
        OU.[OrganizationId] = @OrganizationId
        AND OU.[Status] = 2 -- 2 = Confirmed
        AND (
            CU.[CollectionId] IS NOT NULL
            OR CG.[CollectionId] IS NOT NULL
            OR OU.[AccessAll] = 1
            OR G.[AccessAll] = 1
        )
END
GO
/****** Object:  StoredProcedure [dbo].[User_BumpAccountRevisionDateByEmergencyAccessGranteeId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_BumpAccountRevisionDateByEmergencyAccessGranteeId]
    @EmergencyAccessId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    UPDATE
        U
    SET
        U.[AccountRevisionDate] = GETUTCDATE()
    FROM
        [dbo].[User] U
    INNER JOIN
        [dbo].[EmergencyAccess] EA ON EA.[GranteeId] = U.[Id]
    WHERE
        EA.[Id] = @EmergencyAccessId
        AND EA.[Status] = 2 -- Confirmed
END
GO
/****** Object:  StoredProcedure [dbo].[User_BumpAccountRevisionDateByOrganizationId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_BumpAccountRevisionDateByOrganizationId]
    @OrganizationId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    UPDATE
        U
    SET
        U.[AccountRevisionDate] = GETUTCDATE()
    FROM
        [dbo].[User] U
    INNER JOIN
        [dbo].[OrganizationUser] OU ON OU.[UserId] = U.[Id]
    WHERE
        OU.[OrganizationId] = @OrganizationId
        AND OU.[Status] = 2 -- Confirmed
END
GO
/****** Object:  StoredProcedure [dbo].[User_BumpAccountRevisionDateByOrganizationUserId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_BumpAccountRevisionDateByOrganizationUserId]
    @OrganizationUserId UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    UPDATE
        U
    SET
        U.[AccountRevisionDate] = GETUTCDATE()
    FROM
        [dbo].[User] U
    INNER JOIN
        [dbo].[OrganizationUser] OU ON OU.[UserId] = U.[Id]
    WHERE
        OU.[Id] = @OrganizationUserId
        AND OU.[Status] = 2 -- Confirmed
END
GO
/****** Object:  StoredProcedure [dbo].[User_Create]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_Create]
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(50),
    @Email NVARCHAR(50),
    @EmailVerified BIT,
    @MasterPassword NVARCHAR(300),
    @MasterPasswordHint NVARCHAR(50),
    @Culture NVARCHAR(10),
    @SecurityStamp NVARCHAR(50),
    @TwoFactorProviders NVARCHAR(MAX),
    @TwoFactorRecoveryCode NVARCHAR(32),
    @EquivalentDomains NVARCHAR(MAX),
    @ExcludedGlobalEquivalentDomains NVARCHAR(MAX),
    @AccountRevisionDate DATETIME2(7),
    @Key NVARCHAR(MAX),
    @PublicKey NVARCHAR(MAX),
    @PrivateKey NVARCHAR(MAX),
    @Premium BIT,
    @PremiumExpirationDate DATETIME2(7),
    @RenewalReminderDate DATETIME2(7),
    @Storage BIGINT,
    @MaxStorageGb SMALLINT,
    @Gateway TINYINT,
    @GatewayCustomerId VARCHAR(50),
    @GatewaySubscriptionId VARCHAR(50),
    @ReferenceData VARCHAR(MAX),
    @LicenseKey VARCHAR(100),
    @Kdf TINYINT,
    @KdfIterations INT,
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7),
    @ApiKey VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON

    INSERT INTO [dbo].[User]
    (
        [Id],
        [Name],
        [Email],
        [EmailVerified],
        [MasterPassword],
        [MasterPasswordHint],
        [Culture],
        [SecurityStamp],
        [TwoFactorProviders],
        [TwoFactorRecoveryCode],
        [EquivalentDomains],
        [ExcludedGlobalEquivalentDomains],
        [AccountRevisionDate],
        [Key],
        [PublicKey],
        [PrivateKey],
        [Premium],
        [PremiumExpirationDate],
        [RenewalReminderDate],
        [Storage],
        [MaxStorageGb],
        [Gateway],
        [GatewayCustomerId],
        [GatewaySubscriptionId],
        [ReferenceData],
        [LicenseKey],
        [Kdf],
        [KdfIterations],
        [CreationDate],
        [RevisionDate],
        [ApiKey]
    )
    VALUES
    (
        @Id,
        @Name,
        @Email,
        @EmailVerified,
        @MasterPassword,
        @MasterPasswordHint,
        @Culture,
        @SecurityStamp,
        @TwoFactorProviders,
        @TwoFactorRecoveryCode,
        @EquivalentDomains,
        @ExcludedGlobalEquivalentDomains,
        @AccountRevisionDate,
        @Key,
        @PublicKey,
        @PrivateKey,
        @Premium,
        @PremiumExpirationDate,
        @RenewalReminderDate,
        @Storage,
        @MaxStorageGb,
        @Gateway,
        @GatewayCustomerId,
        @GatewaySubscriptionId,
        @ReferenceData,
        @LicenseKey,
        @Kdf,
        @KdfIterations,
        @CreationDate,
        @RevisionDate,
        @ApiKey
    )
END
GO
/****** Object:  StoredProcedure [dbo].[User_DeleteById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_DeleteById]
@Id UNIQUEIDENTIFIER
    WITH RECOMPILE
AS
BEGIN
    SET NOCOUNT ON
    DECLARE @BatchSize INT = 100

    -- Delete ciphers
    WHILE @BatchSize > 0
    BEGIN
        BEGIN TRANSACTION User_DeleteById_Ciphers

        DELETE TOP(@BatchSize)
        FROM
            [dbo].[Cipher]
        WHERE
            [UserId] = @Id

        SET @BatchSize = @@ROWCOUNT

        COMMIT TRANSACTION User_DeleteById_Ciphers
    END

    BEGIN TRANSACTION User_DeleteById

    -- Delete folders
    DELETE
    FROM
        [dbo].[Folder]
    WHERE
        [UserId] = @Id

    -- Delete devices
    DELETE
    FROM
        [dbo].[Device]
    WHERE
        [UserId] = @Id

    -- Delete collection users
    DELETE
        CU
    FROM
        [dbo].[CollectionUser] CU
    INNER JOIN
        [dbo].[OrganizationUser] OU ON OU.[Id] = CU.[OrganizationUserId]
    WHERE
        OU.[UserId] = @Id

    -- Delete group users
    DELETE
        GU
    FROM
        [dbo].[GroupUser] GU
    INNER JOIN
        [dbo].[OrganizationUser] OU ON OU.[Id] = GU.[OrganizationUserId]
    WHERE
        OU.[UserId] = @Id

    -- Delete organization users
    DELETE
    FROM
        [dbo].[OrganizationUser]
    WHERE
        [UserId] = @Id

    -- Delete U2F logins
    DELETE
    FROM
        [dbo].[U2f]
    WHERE
        [UserId] = @Id

    -- Delete SSO Users
    DELETE
    FROM
        [dbo].[SsoUser]
    WHERE
        [UserId] = @Id

    -- Delete Emergency Accesses
    DELETE
    FROM
        [dbo].[EmergencyAccess]
    WHERE
        [GrantorId] = @Id
    OR
        [GranteeId] = @Id

    -- Finally, delete the user
    DELETE
    FROM
        [dbo].[User]
    WHERE
        [Id] = @Id

    COMMIT TRANSACTION User_DeleteById
END
GO
/****** Object:  StoredProcedure [dbo].[User_ReadAccountRevisionDateById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_ReadAccountRevisionDateById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        [AccountRevisionDate]
    FROM
        [dbo].[User]
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[User_ReadByEmail]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_ReadByEmail]
    @Email NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[UserView]
    WHERE
        [Email] = @Email
END
GO
/****** Object:  StoredProcedure [dbo].[User_ReadById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_ReadById]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[UserView]
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[User_ReadByPremium]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_ReadByPremium]
    @Premium BIT
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        *
    FROM
        [dbo].[UserView]
    WHERE
        [Premium] = @Premium
END
GO
/****** Object:  StoredProcedure [dbo].[User_ReadBySsoUserOrganizationIdExternalId]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_ReadBySsoUserOrganizationIdExternalId]
    @OrganizationId UNIQUEIDENTIFIER,
    @ExternalId NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        U.*
    FROM
        [dbo].[UserView] U
    INNER JOIN
        [dbo].[SsoUser] SU ON SU.[UserId] = U.[Id]
    WHERE
        (
            (@OrganizationId IS NULL AND SU.[OrganizationId] IS NULL)
            OR (@OrganizationId IS NOT NULL AND SU.[OrganizationId] = @OrganizationId)
        )
        AND SU.[ExternalId] = @ExternalId
END
GO
/****** Object:  StoredProcedure [dbo].[User_ReadKdfByEmail]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_ReadKdfByEmail]
    @Email NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        [Kdf],
        [KdfIterations]
    FROM
        [dbo].[User]
    WHERE
        [Email] = @Email
END
GO
/****** Object:  StoredProcedure [dbo].[User_ReadPublicKeyById]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_ReadPublicKeyById]
    @Id NVARCHAR(50)
AS
BEGIN
    SET NOCOUNT ON

    SELECT
        [PublicKey]
    FROM
        [dbo].[User]
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[User_Search]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_Search]
    @Email NVARCHAR(50),
    @Skip INT = 0,
    @Take INT = 25
WITH RECOMPILE
AS
BEGIN
    SET NOCOUNT ON
    DECLARE @EmailLikeSearch NVARCHAR(55) = @Email + '%'

    SELECT
        *
    FROM
        [dbo].[UserView]
    WHERE
        (@Email IS NULL OR [Email] LIKE @EmailLikeSearch)
    ORDER BY [Email] ASC
    OFFSET @Skip ROWS
    FETCH NEXT @Take ROWS ONLY
END
GO
/****** Object:  StoredProcedure [dbo].[User_Update]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_Update]
    @Id UNIQUEIDENTIFIER,
    @Name NVARCHAR(50),
    @Email NVARCHAR(50),
    @EmailVerified BIT,
    @MasterPassword NVARCHAR(300),
    @MasterPasswordHint NVARCHAR(50),
    @Culture NVARCHAR(10),
    @SecurityStamp NVARCHAR(50),
    @TwoFactorProviders NVARCHAR(MAX),
    @TwoFactorRecoveryCode NVARCHAR(32),
    @EquivalentDomains NVARCHAR(MAX),
    @ExcludedGlobalEquivalentDomains NVARCHAR(MAX),
    @AccountRevisionDate DATETIME2(7),
    @Key NVARCHAR(MAX),
    @PublicKey NVARCHAR(MAX),
    @PrivateKey NVARCHAR(MAX),
    @Premium BIT,
    @PremiumExpirationDate DATETIME2(7),
    @RenewalReminderDate DATETIME2(7),
    @Storage BIGINT,
    @MaxStorageGb SMALLINT,
    @Gateway TINYINT,
    @GatewayCustomerId VARCHAR(50),
    @GatewaySubscriptionId VARCHAR(50),
    @ReferenceData VARCHAR(MAX),
    @LicenseKey VARCHAR(100),
    @Kdf TINYINT,
    @KdfIterations INT,
    @CreationDate DATETIME2(7),
    @RevisionDate DATETIME2(7),
    @ApiKey VARCHAR(30)
AS
BEGIN
    SET NOCOUNT ON

    UPDATE
        [dbo].[User]
    SET
        [Name] = @Name,
        [Email] = @Email,
        [EmailVerified] = @EmailVerified,
        [MasterPassword] = @MasterPassword,
        [MasterPasswordHint] = @MasterPasswordHint,
        [Culture] = @Culture,
        [SecurityStamp] = @SecurityStamp,
        [TwoFactorProviders] = @TwoFactorProviders,
        [TwoFactorRecoveryCode] = @TwoFactorRecoveryCode,
        [EquivalentDomains] = @EquivalentDomains,
        [ExcludedGlobalEquivalentDomains] = @ExcludedGlobalEquivalentDomains,
        [AccountRevisionDate] = @AccountRevisionDate,
        [Key] = @Key,
        [PublicKey] = @PublicKey,
        [PrivateKey] = @PrivateKey,
        [Premium] = @Premium,
        [PremiumExpirationDate] = @PremiumExpirationDate,
        [RenewalReminderDate] = @RenewalReminderDate,
        [Storage] = @Storage,
        [MaxStorageGb] = @MaxStorageGb,
        [Gateway] = @Gateway,
        [GatewayCustomerId] = @GatewayCustomerId,
        [GatewaySubscriptionId] = @GatewaySubscriptionId,
        [ReferenceData] = @ReferenceData,
        [LicenseKey] = @LicenseKey,
        [Kdf] = @Kdf,
        [KdfIterations] = @KdfIterations,
        [CreationDate] = @CreationDate,
        [RevisionDate] = @RevisionDate,
        [ApiKey] = @ApiKey
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[User_UpdateKeys]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_UpdateKeys]
    @Id UNIQUEIDENTIFIER,
    @SecurityStamp NVARCHAR(50),
    @Key NVARCHAR(MAX),
    @PrivateKey VARCHAR(MAX),
    @RevisionDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    UPDATE
        [dbo].[User]
    SET
        [SecurityStamp] = @SecurityStamp,
        [Key] = @Key,
        [PrivateKey] = @PrivateKey,
        [RevisionDate] = @RevisionDate,
        [AccountRevisionDate] = @RevisionDate
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[User_UpdateRenewalReminderDate]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_UpdateRenewalReminderDate]
    @Id UNIQUEIDENTIFIER,
    @RenewalReminderDate DATETIME2(7)
AS
BEGIN
    SET NOCOUNT ON

    UPDATE
        [dbo].[User]
    SET
        [RenewalReminderDate] = @RenewalReminderDate
    WHERE
        [Id] = @Id
END
GO
/****** Object:  StoredProcedure [dbo].[User_UpdateStorage]    Script Date: 16/06/2021 10:30:53 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[User_UpdateStorage]
    @Id UNIQUEIDENTIFIER
AS
BEGIN
    SET NOCOUNT ON

    DECLARE @AttachmentStorage BIGINT
    DECLARE @SendStorage BIGINT

    CREATE TABLE #UserStorageUpdateTemp
    ( 
        [Id] UNIQUEIDENTIFIER NOT NULL,
        [Attachments] VARCHAR(MAX) NULL
    )

    INSERT INTO #UserStorageUpdateTemp
    SELECT
        [Id],
        [Attachments]
    FROM
        [dbo].[Cipher]
    WHERE
        [UserId] = @Id

    ;WITH [CTE] AS (
        SELECT
            [Id],
            (
                SELECT
                    SUM(CAST(JSON_VALUE(value,'$.Size') AS BIGINT))
                FROM
                    OPENJSON([Attachments])
            ) [Size]
        FROM
            #UserStorageUpdateTemp
    )
    SELECT
        @AttachmentStorage = SUM([CTE].[Size])
    FROM
        [CTE]

    DROP TABLE #UserStorageUpdateTemp

    ;WITH [CTE] AS (
        SELECT
            [Id],
            CAST(JSON_VALUE([Data],'$.Size') AS BIGINT) [Size]
        FROM
            [Send]
        WHERE
            [UserId] = @Id
    )
    SELECT
        @SendStorage = SUM([CTE].[Size])
    FROM
        [CTE]

    UPDATE
        [dbo].[User]
    SET
        [Storage] = (ISNULL(@AttachmentStorage, 0) + ISNULL(@SendStorage, 0)),
        [RevisionDate] = GETUTCDATE()
    WHERE
        [Id] = @Id
END
GO
USE [master]
GO
ALTER DATABASE [vault] SET  READ_WRITE 
GO
