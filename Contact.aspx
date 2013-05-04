<%@ Page Title="Contact" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="ScheduleGeneratorProject.Contact" %>

<asp:Content runat="server" ID="BodyContent" ContentPlaceHolderID="MainContent">
    <hgroup class="title">
        <h1><%: Title %>.</h1>
    </hgroup>

    <section class="contact">
        <header>
            <h3>Email:</h3>
        </header>
        <p>
            <span class="label">CEO:</span>
            <span><a href="mailto:rjhender@ku.edu">rjhender@ku.edu</a></span>
        </p>
        <p>
            <span class="label">COO:</span>
            <span><a href="mailto:lonniej@ku.edu">lonniej@ku.edu</a></span>
        </p>
        <p>
            <span class="label">CFO:</span>
            <span><a href="mailto:gzli@ku.edu">gzli@ku.edu</a></span>
        </p>
        <p>
            <span class="label">CTO:</span>
            <span><a href="mailto:adonnell@ku.edu">adonnell@ku.edu</a></span>
        </p>
    </section>

    <section class="contact">
        <header>
            <h3>Address:</h3>
        </header>
        <p>
            1520 West 15th Street<br />
            2001 Eaton Hall<br />
            University of Kansas<br />
            Lawrence, KS 66045-7608
        </p>
    </section>
</asp:Content>