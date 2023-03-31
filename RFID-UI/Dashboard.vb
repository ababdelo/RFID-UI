
Imports MySql.Data.MySqlClient

Public Class Dashboard

    'server=localhost; user=yout_database_user; password=your_database_password; database=your_database_name
    Dim Connection As New MySqlConnection("server=localhost; user=root; password='Thewayofthemakersis_1337'; database=rfid_user_data")
    Dim MySQLCMD As New MySqlCommand
    Dim MySQLDA As New MySqlDataAdapter
    Dim DT As New DataTable
    Dim Table_Name As String = "rfid_user_data_table" 'your table name
    Dim Data As Integer

    Dim LoadImagesStr As Boolean = False
    Dim IDRam As String
    Dim IMG_FileNameInput As String
    Dim StatusInput As String = "Save"
    Dim SqlCmdSearchstr As String

    Public Shared StrSerialIn As String
    Dim GetID As Boolean = False
    Dim ViewUserData As Boolean = False

    Private Sub Dashboard_Load(sender As Object, e As EventArgs) Handles MyBase.Load
        Me.CenterToScreen()
        PanelReadingTagProcess.Visible = False
        PanelRegistrationandEditUserData.Visible = False
        PanelUserData.Visible = False
        PanelConnection.Visible = True
        ComboBoxBaudRate.SelectedIndex = 3
        TimerSerialIn.Interval = 500
    End Sub
    Private Sub ShowData()
        Try
            Connection.Open()
        Catch ex As Exception
            MessageBox.Show("Connection failed !!!" & vbCrLf & "Please check that the server is ready !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        Try
            If LoadImagesStr = False Then
                MySQLCMD.CommandType = CommandType.Text
                MySQLCMD.CommandText = "SELECT ID, Name, Address, Contact, Role FROM " & Table_Name & " ORDER BY Name"
                MySQLDA = New MySqlDataAdapter(MySQLCMD.CommandText, Connection)
                DT = New DataTable
                Data = MySQLDA.Fill(DT)
                If Data > 0 Then
                    DataGridView.DataSource = Nothing
                    DataGridView.DataSource = DT
                    DataGridView.Columns(2).DefaultCellStyle.Format = "c"
                    DataGridView.DefaultCellStyle.ForeColor = Color.Black
                    DataGridView.ClearSelection()
                Else
                    DataGridView.DataSource = DT
                End If
            Else
                MySQLCMD.CommandType = CommandType.Text
                MySQLCMD.CommandText = "SELECT PPicture FROM " & Table_Name & " WHERE ID LIKE '" & IDRam & "'"
                MySQLDA = New MySqlDataAdapter(MySQLCMD.CommandText, Connection)
                DT = New DataTable
                Data = MySQLDA.Fill(DT)
                If Data > 0 Then
                    Dim ImgArray() As Byte = DT.Rows(0).Item("PPicture")
                    Dim lmgStr As New System.IO.MemoryStream(ImgArray)
                    PictureBoxImagePreview.Image = Image.FromStream(lmgStr)
                    PictureBoxImagePreview.SizeMode = PictureBoxSizeMode.Zoom
                    lmgStr.Close()
                End If
                LoadImagesStr = False
            End If
        Catch ex As Exception
            MsgBox("Failed to load Database !!!" & vbCr & ex.Message, MsgBoxStyle.Critical, "Error Message")
            Connection.Close()
            Return
        End Try

        DT = Nothing
        Connection.Close()
    End Sub

    Private Sub ShowDataUser()
        Try
            Connection.Open()
        Catch ex As Exception
            MessageBox.Show("Connection failed !!!" & vbCrLf & "Please check that the server is ready !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        Try
            MySQLCMD.CommandType = CommandType.Text
            MySQLCMD.CommandText = "SELECT * FROM " & Table_Name & " WHERE ID LIKE '" & LabelID.Text.Substring(5, LabelID.Text.Length - 5) & "'"
            MySQLDA = New MySqlDataAdapter(MySQLCMD.CommandText, Connection)
            DT = New DataTable
            Data = MySQLDA.Fill(DT)
            If Data > 0 Then
                Dim ImgArray() As Byte = DT.Rows(0).Item("PPicture")
                Dim lmgStr As New System.IO.MemoryStream(ImgArray)
                PictureBoxUserImage.Image = Image.FromStream(lmgStr)
                lmgStr.Close()

                LabelID.Text = "ID : " & DT.Rows(0).Item("ID")
                LabelName.Text = DT.Rows(0).Item("Name")
                LabelAddress.Text = DT.Rows(0).Item("Address")
                LabelContact.Text = DT.Rows(0).Item("Contact")
                LabelRole.Text = DT.Rows(0).Item("Role")
            Else
                MsgBox("ID not found !!!" & vbCr & "Please register your ID.", MsgBoxStyle.Information, "Information Message")
            End If
        Catch ex As Exception
            MsgBox("Failed to load Database !!!" & vbCr & ex.Message, MsgBoxStyle.Critical, "Error Message")
            Connection.Close()
            Return
        End Try

        DT = Nothing
        Connection.Close()
    End Sub

    Private Sub ClearInputUpdateData()
        TextBoxName.Text = ""
        LabelGetID.Text = "________"
        TextBoxAddress.Text = ""
        TextBoxContact.Text = ""
        TextBoxRole.Text = ""
        PictureBoxImageInput.Image = My.Resources.Click_to_browse
    End Sub

    Private Sub ButtonConnection_Click(sender As Object, e As EventArgs) Handles ButtonConnection.Click
        PictureBoxSelect.Top = ButtonConnection.Top
        PanelUserData.Visible = False
        PanelRegistrationandEditUserData.Visible = False
        PanelConnection.Visible = True
    End Sub

    Private Sub ButtonUserData_Click(sender As Object, e As EventArgs) Handles ButtonUserData.Click
        If TimerSerialIn.Enabled = False Then
            MsgBox("Failed to open User Data !!!" & vbCr & "Click the Connection menu then click the Connect button.", MsgBoxStyle.Information, "Information")
            Return
        Else
            StrSerialIn = ""
            ViewUserData = True
            PictureBoxSelect.Top = ButtonUserData.Top
            PanelRegistrationandEditUserData.Visible = False
            PanelConnection.Visible = False
            PanelUserData.Visible = True
        End If
    End Sub

    Private Sub ButtonRegistration_Click(sender As Object, e As EventArgs) Handles ButtonRegistration.Click
        StrSerialIn = ""
        ViewUserData = False
        PictureBoxSelect.Top = ButtonRegistration.Top
        PanelConnection.Visible = False
        PanelUserData.Visible = False
        PanelRegistrationandEditUserData.Visible = True
        ShowData()
    End Sub

    Private Sub PanelConnection_Paint(sender As Object, e As PaintEventArgs) Handles PanelConnection.Paint
        e.Graphics.DrawRectangle(New Pen(Color.LightGray, 2), PanelConnection.ClientRectangle)
    End Sub

    Private Sub PanelConnection_Resize(sender As Object, e As EventArgs) Handles PanelConnection.Resize
        PanelConnection.Invalidate()
    End Sub

    Private Sub PanelUserData_Paint(sender As Object, e As PaintEventArgs) Handles PanelUserData.Paint
        e.Graphics.DrawRectangle(New Pen(Color.LightGray, 2), PanelUserData.ClientRectangle)
    End Sub

    Private Sub PanelUserData_Resize(sender As Object, e As EventArgs) Handles PanelUserData.Resize
        PanelConnection.Invalidate()
    End Sub

    Private Sub PanelRegistrationandEditUserData_Paint(sender As Object, e As PaintEventArgs) Handles PanelRegistrationandEditUserData.Paint
        e.Graphics.DrawRectangle(New Pen(Color.LightGray, 2), PanelRegistrationandEditUserData.ClientRectangle)
    End Sub

    Private Sub PanelRegistrationandEditUserData_Resize(sender As Object, e As EventArgs) Handles PanelRegistrationandEditUserData.Resize
        PanelConnection.Invalidate()
    End Sub

    Private Sub ButtonScanPort_Click(sender As Object, e As EventArgs) Handles ButtonScanPort.Click
        ComboBoxPort.Items.Clear()
        Dim myPort As Array
        Dim i As Integer
        myPort = IO.Ports.SerialPort.GetPortNames()
        ComboBoxPort.Items.AddRange(myPort)
        i = ComboBoxPort.Items.Count
        i = i - i
        Try
            ComboBoxPort.SelectedIndex = i
        Catch ex As Exception
            MsgBox("Com port not detected", MsgBoxStyle.Critical, "Error Message")
            ComboBoxPort.Text = ""
            ComboBoxPort.Items.Clear()
            Return
        End Try
        ComboBoxPort.DroppedDown = True
    End Sub

    Private Sub ButtonScanPort_MouseHover(sender As Object, e As EventArgs) Handles ButtonScanPort.MouseHover
        ButtonScanPort.ForeColor = Color.White
    End Sub

    Private Sub ButtonScanPort_Leave(sender As Object, e As EventArgs) Handles ButtonScanPort.Leave
        ButtonScanPort.ForeColor = Color.FromArgb(0, 70, 180)
    End Sub

    Private Sub ButtonConnect_Click(sender As Object, e As EventArgs) Handles ButtonConnect.Click
        If ButtonConnect.Text = "Connect" Then
            SerialPort.BaudRate = ComboBoxBaudRate.SelectedItem
            SerialPort.PortName = ComboBoxPort.SelectedItem
            Try
                SerialPort.Open()
                TimerSerialIn.Start()
                ButtonConnect.Text = "Disconnect"
                PictureBoxStatusConnect.Image = My.Resources.Connected
            Catch ex As Exception
                MsgBox("Failed to connect !!!" & vbCr & "Arduino is not detected.", MsgBoxStyle.Critical, "Error Message")
                PictureBoxStatusConnect.Image = My.Resources.Disconnected
            End Try
        ElseIf ButtonConnect.Text = "Disconnect" Then
            PictureBoxStatusConnect.Image = My.Resources.Disconnected
            ButtonConnect.Text = "Connect"
            LabelConnectionStatus.Text = "Connection Status : Disconnect"
            TimerSerialIn.Stop()
            SerialPort.Close()
        End If
    End Sub

    Private Sub ButtonConnect_MouseHover(sender As Object, e As EventArgs) Handles ButtonConnect.MouseHover
        ButtonConnect.ForeColor = Color.White
    End Sub

    Private Sub ButtonConnect_Leave(sender As Object, e As EventArgs) Handles ButtonConnect.Leave
        ButtonConnect.ForeColor = Color.FromArgb(0, 70, 180)
    End Sub

    Private Sub ButtonClear_Click(sender As Object, e As EventArgs) Handles ButtonClear.Click
        LabelID.Text = "ID : ________"
        LabelName.Text = "Waiting ..."
        LabelAddress.Text = "Waiting ..."
        LabelContact.Text = "Waiting ..."
        LabelRole.Text = "Waiting ..."
        PictureBoxUserImage.Image = Nothing
    End Sub
    Private Sub ButtonClear_MouseHover(sender As Object, e As EventArgs) Handles ButtonClear.MouseHover
        ButtonClear.ForeColor = Color.White
    End Sub

    Private Sub ButtonClear_MouseLeave(sender As Object, e As EventArgs) Handles ButtonClear.MouseLeave
        ButtonClear.ForeColor = Color.FromArgb(0, 70, 180)
    End Sub

    Private Sub ButtonSave_Click(sender As Object, e As EventArgs) Handles ButtonSave.Click
        Dim mstream As New System.IO.MemoryStream()
        Dim arrImage() As Byte

        If TextBoxName.Text = "" Then
            MessageBox.Show("Name cannot be empty !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If TextBoxAddress.Text = "" Then
            MessageBox.Show("Address cannot be empty !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If TextBoxContact.Text = "" Then
            MessageBox.Show("Account cannot be empty !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If TextBoxRole.Text = "" Then
            MessageBox.Show("Role cannot be empty !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End If

        If StatusInput = "Save" Then
            If IMG_FileNameInput <> "" Then
                PictureBoxImageInput.Image.Save(mstream, System.Drawing.Imaging.ImageFormat.Jpeg)
                arrImage = mstream.GetBuffer()
            Else
                MessageBox.Show("The image has not been selected !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End If

            Try
                Connection.Open()
            Catch ex As Exception
                MessageBox.Show("Connection failed !!!" & vbCrLf & "Please check that the server is ready !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
                Return
            End Try

            Try
                MySQLCMD = New MySqlCommand
                With MySQLCMD
                    .CommandText = "INSERT INTO " & Table_Name & " (ID, Name, Address, Contact, Role, PPicture) VALUES (@ID, @name, @address, @contact, @role, @ppicture)"
                    .Connection = Connection
                    .Parameters.AddWithValue("@id", LabelGetID.Text)
                    .Parameters.AddWithValue("@name", TextBoxName.Text)
                    .Parameters.AddWithValue("@address", TextBoxAddress.Text)
                    .Parameters.AddWithValue("@contact", TextBoxContact.Text)
                    .Parameters.AddWithValue("@role", TextBoxRole.Text)
                    .Parameters.AddWithValue("@ppicture", arrImage)
                    .ExecuteNonQuery()
                End With
                MsgBox("Data saved successfully", MsgBoxStyle.Information, "Information")
                IMG_FileNameInput = ""
                ClearInputUpdateData()
            Catch ex As Exception
                MsgBox("Data failed to save !!!" & vbCr & ex.Message, MsgBoxStyle.Critical, "Error Message")
                Connection.Close()
                Return
            End Try
            Connection.Close()

        Else

            If IMG_FileNameInput <> "" Then
                PictureBoxImageInput.Image.Save(mstream, System.Drawing.Imaging.ImageFormat.Jpeg)
                arrImage = mstream.GetBuffer()

                Try
                    Connection.Open()
                Catch ex As Exception
                    MessageBox.Show("Connection failed !!!" & vbCrLf & "Please check that the server is ready !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End Try

                Try
                    MySQLCMD = New MySqlCommand
                    With MySQLCMD
                        .CommandText = "UPDATE " & Table_Name & " SET  ID=@id,Name=@name,Address=@address,Contact=@contact,Role=@role,PPicture=@ppicture WHERE ID=@id "
                        .Connection = Connection
                        .Parameters.AddWithValue("@id", LabelGetID.Text)
                        .Parameters.AddWithValue("@name", TextBoxName.Text)
                        .Parameters.AddWithValue("@address", TextBoxAddress.Text)
                        .Parameters.AddWithValue("@contact", TextBoxContact.Text)
                        .Parameters.AddWithValue("@role", TextBoxRole.Text)
                        .Parameters.AddWithValue("@PPicture", arrImage)
                        .ExecuteNonQuery()
                    End With
                    MsgBox("Data updated successfully", MsgBoxStyle.Information, "Information")
                    IMG_FileNameInput = ""
                    ButtonSave.Text = "Save"
                    ClearInputUpdateData()
                Catch ex As Exception
                    MsgBox("Data failed to Update !!!" & vbCr & ex.Message, MsgBoxStyle.Critical, "Error Message")
                    Connection.Close()
                    Return
                End Try
                Connection.Close()

            Else

                Try
                    Connection.Open()
                Catch ex As Exception
                    MessageBox.Show("Connection failed !!!" & vbCrLf & "Please check that the server is ready !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
                    Return
                End Try

                Try
                    MySQLCMD = New MySqlCommand
                    With MySQLCMD
                        .CommandText = "UPDATE " & Table_Name & " SET  ID=@id,Name=@name,Address=@address,Contact=@contact,Role=@role WHERE ID=@id "
                        .Connection = Connection
                        .Parameters.AddWithValue("@id", LabelGetID.Text)
                        .Parameters.AddWithValue("@name", TextBoxName.Text)
                        .Parameters.AddWithValue("@address", TextBoxAddress.Text)
                        .Parameters.AddWithValue("@contact", TextBoxContact.Text)
                        .Parameters.AddWithValue("@role", TextBoxRole.Text)
                        .ExecuteNonQuery()
                    End With
                    MsgBox("Data updated successfully", MsgBoxStyle.Information, "Information")
                    ButtonSave.Text = "Save"
                    ClearInputUpdateData()
                Catch ex As Exception
                    MsgBox("Data failed to Update !!!" & vbCr & ex.Message, MsgBoxStyle.Critical, "Error Message")
                    Connection.Close()
                    Return
                End Try
                Connection.Close()
            End If
            StatusInput = "Save"
        End If
        PictureBoxImagePreview.Image = Nothing
        ShowData()
    End Sub

    Private Sub ButtonSave_MouseHover(sender As Object, e As EventArgs) Handles ButtonSave.MouseHover
        ButtonSave.ForeColor = Color.White
    End Sub

    Private Sub ButtonSave_MouseLeave(sender As Object, e As EventArgs) Handles ButtonSave.MouseLeave
        ButtonSave.ForeColor = Color.FromArgb(0, 70, 180)
    End Sub

    Private Sub ButtonClearForm_MouseHover(sender As Object, e As EventArgs) Handles ButtonClearForm.MouseHover
        ButtonClearForm.ForeColor = Color.White
    End Sub

    Private Sub ButtonClearForm_MouseLeave(sender As Object, e As EventArgs) Handles ButtonClearForm.MouseLeave
        ButtonClearForm.ForeColor = Color.FromArgb(0, 70, 180)
    End Sub

    Private Sub ButtonClearForm_Click(sender As Object, e As EventArgs) Handles ButtonClearForm.Click
        ClearInputUpdateData()
    End Sub

    Private Sub ButtonScanID_Click(sender As Object, e As EventArgs) Handles ButtonScanID.Click
        If TimerSerialIn.Enabled = True Then
            PanelReadingTagProcess.Visible = True
            GetID = True
            ButtonScanID.Enabled = False
        Else
            MsgBox("Failed to open User Data !!!" & vbCr & "Click the Connection menu then click the Connect button.", MsgBoxStyle.Critical, "Error Message")
        End If
    End Sub

    Private Sub ButtonScanID_MouseHover(sender As Object, e As EventArgs) Handles ButtonScanID.MouseHover
        ButtonScanID.ForeColor = Color.White
    End Sub

    Private Sub ButtonScanID_MouseLeave(sender As Object, e As EventArgs) Handles ButtonScanID.MouseLeave
        ButtonScanID.ForeColor = Color.FromArgb(0, 70, 180)
    End Sub

    Private Sub PictureBoxImageInput_Click(sender As Object, e As EventArgs) Handles PictureBoxImageInput.Click
        OpenFileDialog.FileName = ""
        OpenFileDialog.Filter = "JPEG/PNG (*.jpeg;*.jpg;*.png) | *.jpeg;*.jpg;*.png"

        If (OpenFileDialog.ShowDialog(Me) = System.Windows.Forms.DialogResult.OK) Then
            IMG_FileNameInput = OpenFileDialog.FileName
            PictureBoxImageInput.ImageLocation = IMG_FileNameInput
        End If
    End Sub

    Private Sub CheckBoxByName_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxByName.CheckedChanged
        If CheckBoxByName.Checked = True Then
            CheckBoxByID.Checked = False
        End If
        If CheckBoxByName.Checked = False Then
            CheckBoxByID.Checked = True
        End If
    End Sub

    Private Sub CheckBoxByID_CheckedChanged(sender As Object, e As EventArgs) Handles CheckBoxByID.CheckedChanged
        If CheckBoxByID.Checked = True Then
            CheckBoxByName.Checked = False
        End If
        If CheckBoxByID.Checked = False Then
            CheckBoxByName.Checked = True
        End If
    End Sub

    Private Sub TextBoxSearch_TextChanged(sender As Object, e As EventArgs) Handles TextBoxSearch.TextChanged
        If CheckBoxByID.Checked = True Then
            If TextBoxSearch.Text = Nothing Then
                SqlCmdSearchstr = "SELECT ID, Name, Address, Contact, Role FROM " & Table_Name & " ORDER BY Name"
            Else
                SqlCmdSearchstr = "SELECT ID, Name, Address, Contact, Role FROM " & Table_Name & " WHERE ID LIKE'" & TextBoxSearch.Text & "%'"
            End If
        End If
        If CheckBoxByName.Checked = True Then
            If TextBoxSearch.Text = Nothing Then
                SqlCmdSearchstr = "SELECT ID, Name, Address, Contact, Role FROM " & Table_Name & " ORDER BY Name"
            Else
                SqlCmdSearchstr = "SELECT Name, ID, Address, Contact, Role FROM " & Table_Name & " WHERE Name LIKE'" & TextBoxSearch.Text & "%'"
            End If
        End If

        Try
            Connection.Open()
        Catch ex As Exception
            MessageBox.Show("Connection failed !!!" & vbCrLf & "Please check that the server is ready !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        Try
            MySQLDA = New MySqlDataAdapter(SqlCmdSearchstr, Connection)
            DT = New DataTable
            Data = MySQLDA.Fill(DT)
            If Data > 0 Then
                DataGridView.DataSource = Nothing
                DataGridView.DataSource = DT
                DataGridView.DefaultCellStyle.ForeColor = Color.Black
                DataGridView.ClearSelection()
            Else
                DataGridView.DataSource = DT
            End If
        Catch ex As Exception
            MsgBox("Failed to search" & vbCr & ex.Message, MsgBoxStyle.Critical, "Error Message")
            Connection.Close()
        End Try
        Connection.Close()
    End Sub

    Private Sub DataGridView1_CellMouseDown(sender As Object, e As DataGridViewCellMouseEventArgs) Handles DataGridView.CellMouseDown
        Try
            If AllCellsSelected(DataGridView) = False Then
                If e.Button = MouseButtons.Left Then
                    DataGridView.CurrentCell = DataGridView(e.ColumnIndex, e.RowIndex)
                    Dim i As Integer
                    With DataGridView
                        If e.RowIndex >= 0 Then
                            i = .CurrentRow.Index
                            LoadImagesStr = True
                            IDRam = .Rows(i).Cells("ID").Value.ToString
                            ShowData()
                        End If
                    End With
                End If
            End If
        Catch ex As Exception
            Return
        End Try
    End Sub

    Private Function AllCellsSelected(dgv As DataGridView) As Boolean
        AllCellsSelected = (DataGridView.SelectedCells.Count = (DataGridView.RowCount * DataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible)))
    End Function

    Private Sub TimerTimeDate_Tick(sender As Object, e As EventArgs) Handles TimerTimeDate.Tick
        LabelDateTime.Text = "Time " & DateTime.Now.ToString("HH:mm:ss") & "  Date " & DateTime.Now.ToString("dd MMM, yyyy")
    End Sub

    Private Sub DeleteToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles DeleteToolStripMenuItem.Click
        If DataGridView.RowCount = 0 Then
            MsgBox("Cannot delete, table data is empty", MsgBoxStyle.Critical, "Error Message")
            Return
        End If

        If DataGridView.SelectedRows.Count = 0 Then
            MsgBox("Cannot delete, select the table data to be deleted", MsgBoxStyle.Critical, "Error Message")
            Return
        End If

        If MsgBox("Delete record?", MsgBoxStyle.Question + MsgBoxStyle.OkCancel, "Confirmation") = MsgBoxResult.Cancel Then
            Return
        End If

        Try
            Connection.Open()
        Catch ex As Exception
            MessageBox.Show("Connection failed !!!" & vbCrLf & "Please check that the server is ready !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        Try
            If AllCellsSelected(DataGridView) = True Then
                MySQLCMD.CommandType = CommandType.Text
                MySQLCMD.CommandText = "DELETE FROM " & Table_Name
                MySQLCMD.Connection = Connection
                MySQLCMD.ExecuteNonQuery()
            End If

            For Each row As DataGridViewRow In DataGridView.SelectedRows
                If row.Selected = True Then
                    MySQLCMD.CommandType = CommandType.Text
                    MySQLCMD.CommandText = "DELETE FROM " & Table_Name & " WHERE ID='" & row.DataBoundItem(1).ToString & "'"
                    MySQLCMD.Connection = Connection
                    MySQLCMD.ExecuteNonQuery()
                End If
            Next
        Catch ex As Exception
            MsgBox("Failed to delete" & vbCr & ex.Message, MsgBoxStyle.Critical, "Error Message")
            Connection.Close()
        End Try
        PictureBoxImagePreview.Image = Nothing
        Connection.Close()
        ShowData()
    End Sub

    Private Sub SelectAllToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles SelectAllToolStripMenuItem.Click
        DataGridView.SelectAll()
    End Sub

    Private Sub ClearSelectionToolStripMenuItem_Click(sender As Object, e As EventArgs) Handles ClearSelectionToolStripMenuItem.Click
        DataGridView.ClearSelection()
        PictureBoxImagePreview.Image = Nothing
    End Sub

    Private Sub RefreshToolStripMenuItem1_Click(sender As Object, e As EventArgs) Handles RefreshToolStripMenuItem1.Click
        PictureBoxImagePreview.Image = Nothing
        ShowData()
    End Sub

    Private Sub TimerSerialIn_Tick(sender As Object, e As EventArgs) Handles TimerSerialIn.Tick
        Try
            StrSerialIn = SerialPort.ReadExisting
            LabelConnectionStatus.Text = "Connection Status : Connected"
            If StrSerialIn <> "" Then
                If GetID = True Then
                    LabelGetID.Text = StrSerialIn
                    GetID = False
                    If LabelGetID.Text <> "________" Then
                        PanelReadingTagProcess.Visible = False
                        IDCheck()
                    End If
                End If
                If ViewUserData = True Then
                    ViewData()
                End If
            End If
        Catch ex As Exception
            TimerSerialIn.Stop()
            SerialPort.Close()
            LabelConnectionStatus.Text = "Connection Status : Disconnect"
            PictureBoxStatusConnect.Image = My.Resources.Disconnected
            MsgBox("Failed to connect !!!" & vbCr & "Arduino is not detected.", MsgBoxStyle.Critical, "Error Message")
            ButtonConnect_Click(sender, e)
            Return
        End Try

        If PictureBoxStatusConnect.Visible = True Then
            PictureBoxStatusConnect.Visible = False
        ElseIf PictureBoxStatusConnect.Visible = False Then
            PictureBoxStatusConnect.Visible = True
        End If
    End Sub

    Private Sub IDCheck()
        Try
            Connection.Open()
        Catch ex As Exception
            MessageBox.Show("Connection failed !!!" & vbCrLf & "Please check that the server is ready !!!", "Error Message", MessageBoxButtons.OK, MessageBoxIcon.Error)
            Return
        End Try

        Try
            MySQLCMD.CommandType = CommandType.Text
            MySQLCMD.CommandText = "SELECT * FROM " & Table_Name & " WHERE ID LIKE '" & LabelGetID.Text & "'"
            MySQLDA = New MySqlDataAdapter(MySQLCMD.CommandText, Connection)
            DT = New DataTable
            Data = MySQLDA.Fill(DT)
            If Data > 0 Then
                If MsgBox("ID registered !" & vbCr & "Do you want to edit the data ?", MsgBoxStyle.Question + MsgBoxStyle.OkCancel, "Confirmation") = MsgBoxResult.Cancel Then
                    DT = Nothing
                    Connection.Close()
                    ButtonScanID.Enabled = True
                    GetID = False
                    LabelGetID.Text = "________"
                    Return
                Else
                    Dim ImgArray() As Byte = DT.Rows(0).Item("PPicture")
                    Dim lmgStr As New System.IO.MemoryStream(ImgArray)
                    PictureBoxImageInput.Image = Image.FromStream(lmgStr)
                    PictureBoxImageInput.SizeMode = PictureBoxSizeMode.Zoom

                    TextBoxName.Text = DT.Rows(0).Item("Name")
                    TextBoxAddress.Text = DT.Rows(0).Item("Address")
                    TextBoxContact.Text = DT.Rows(0).Item("Contact")
                    TextBoxRole.Text = DT.Rows(0).Item("Role")
                    StatusInput = "Update"
                End If
            End If
        Catch ex As Exception
            MsgBox("Failed to load Database !!!" & vbCr & ex.Message, MsgBoxStyle.Critical, "Error Message")
            Connection.Close()
            Return
        End Try

        DT = Nothing
        Connection.Close()

        ButtonScanID.Enabled = True
        GetID = False
    End Sub

    Private Sub ViewData()
        LabelID.Text = "ID : " & StrSerialIn
        If LabelID.Text = "ID : ________" Then
            ViewData()
        Else
            ShowDataUser()
        End If
    End Sub
    Private Sub Form1_Resize(sender As Object, e As EventArgs) Handles Me.Resize
        ' GroupBoxImage.Location = New Point((PanelUserData.Width / 5) - (GroupBoxImage.Width / 5), GroupBoxImage.Top)
        PanelReadingTagProcess.Location = New Point((PanelRegistrationandEditUserData.Width / 3) - (PanelReadingTagProcess.Width / 4), 100)
    End Sub

    Private Sub ButtonCloseReadingTag_Click(sender As Object, e As EventArgs) Handles ButtonCloseReadingTag.Click
        PanelReadingTagProcess.Visible = False
        ButtonScanID.Enabled = True
    End Sub

End Class
