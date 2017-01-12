'
' AutomaticCalc VBA macro
'
' Author : Lorenzo Delana <oss.devel@searchathing.com>
'
' The MIT License(MIT)
' Copyright(c) 2016 Lorenzo Delana, https://searchathing.com
'
' Permission is hereby granted, free of charge, to any person obtaining a
' copy of this software and associated documentation files (the "Software"),
' to deal in the Software without restriction, including without limitation
' the rights to use, copy, modify, merge, publish, distribute, sublicense,
' and/or sell copies of the Software, and to permit persons to whom the
' Software is furnished to do so, subject to the following conditions:
'
' The above copyright notice and this permission notice shall be included in
' all copies or substantial portions of the Software.
'
' THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
' IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
' FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
' AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
' LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
' FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER
' DEALINGS IN THE SOFTWARE.
'

Sub AutomaticCalc(data_pathfilename As String, output_pathfilename As String)
	Application.Visible = False
	Application.Calculation = xlCalculationManual
	Application.ScreenUpdating = False
	Application.DisplayStatusBar = False
	Application.EnableEvents = False	
 '   Application.EnableCancelKey = xlDisabled       

    ' create a tmp sheet
    Dim ws_tmp As Worksheet
    Set ws_tmp = ResetSheet("tmp")

    ' load data
    ReadDataIntoSheet ws_tmp, data_pathfilename

    ' compute
    Compute ws_tmp, output_pathfilename

    ' remove tmp sheet
    DeleteSheetIfExists "tmp"

'    Application.EnableCancelKey = xlInterrupt	
	Application.EnableEvents = True
	Application.DisplayStatusBar = True
    Application.ScreenUpdating = True
    Application.Calculation = xlCalculationAutomatic
End Sub

Sub Compute(ws_tmp As Worksheet, output_pathfilename As String)
    ' retrieve last row
    Dim last_row As Long
    last_row = Columns(1).Find("*", , , , xlByColumns, xlPrevious).Row

    Dim ws_input As Worksheet
    Dim input_wsname As String
    Dim input_field_row As Integer
    Dim intput_field_count As Integer
    
    Dim output_wsname As String
    Dim ws_output As Worksheet
    Dim output_field_row As Integer
    Dim output_field_count As Integer
    
    ir = 1
    Do While ir <= last_row
        cmd = ws_tmp.Cells(ir, 1)
        
        '----------------
        ' cells_to_write
        '----------------
        If cmd = "cells_to_write" Then
        
            ' input worksheet
            input_wsname = ws_tmp.Cells(ir, 2)
            Set ws_input = ThisWorkbook.Sheets(input_wsname)
    
            ' input column count
            input_field_row = ir
            input_field_count = ws_tmp.Cells(ir, 3)
        
        '----------------
        ' cells_to_read
        '----------------
        ElseIf cmd = "cells_to_read" Then
        
            ' output worksheet
            output_wsname = ws_tmp.Cells(ir, 2)
            Set ws_output = ThisWorkbook.Sheets(output_wsname)
        
            ' output column count
            output_field_row = ir
            output_field_count = ws_tmp.Cells(ir, 3)
        
        '-----------------------
        ' input_data_set_follow
        '-----------------------
        ElseIf cmd = "input_data_set_follow" Then
        
            ' create a tmp out sheet
            Dim ws_tmp_out As Worksheet
            Set ws_tmp_out = ResetSheet("tmp_out")
            
            tmp_out_row = 1

			ir = ir + 1 ' skip input_data_set_follow line                			

            ' input data loop
            Do While ir <= last_row
                                                                    
                ' write cells
                For input_col = 1 To input_field_count
                    ' retrieve cellname where to place input data
                    input_cellname = ws_tmp.Cells(input_field_row, 3 + input_col)
        
                    ' set data in the input worksheet
                    Value = ws_tmp.Cells(ir, input_col)
                    ws_input.Range(input_cellname).Value = Value
                Next input_col
        
                ' calculate
                Application.Calculate
        
                ' read cells
                For output_col = 1 To output_field_count
                    Dim output_cellname As String
                    output_cellname = ws_tmp.Cells(output_field_row, 3 + output_col)
                    ws_tmp_out.Cells(tmp_out_row, output_col).Value = ws_output.Range(output_cellname).Value
                Next output_col
                tmp_out_row = tmp_out_row + 1
                        
                ir = ir + 1
            Loop
            
            Dim wb_before As Workbook
            Set wb_before = ActiveWorkbook                                			

            ' export result to csv
            ws_tmp_out.Move
            ActiveWorkbook.SaveAs Filename:=output_pathfilename, FileFormat:=xlCSV, CreateBackup:=False, Local:=False			
            ActiveWorkbook.Close
            
            wb_before.Activate
                        
            ' remove tmp out sheet
            DeleteSheetIfExists "tmp_out"			
        End If
    
        ir = ir + 1
    Loop
    
End Sub

Sub ReadDataIntoSheet(ws As Worksheet, pathfilename As String)
    ' import pathfilename csv into given worksheet
    With ws.QueryTables.Add(Connection:="TEXT;" & pathfilename, Destination:=Range("$A$1"))
        .Name = "CAPTURE"
        .FieldNames = True
        .RowNumbers = False
        .FillAdjacentFormulas = False
        .PreserveFormatting = True
        .RefreshOnFileOpen = False
        .RefreshStyle = xlInsertDeleteCells
        .SavePassword = False
        .SaveData = True
        .AdjustColumnWidth = True
        .RefreshPeriod = 0
        .TextFilePromptOnRefresh = False
        .TextFilePlatform = 437
        .TextFileStartRow = 1
        .TextFileParseType = xlDelimited
        .TextFileTextQualifier = xlTextQualifierDoubleQuote
        .TextFileConsecutiveDelimiter = False
        .TextFileTabDelimiter = True
        .TextFileSemicolonDelimiter = False
        .TextFileCommaDelimiter = True
        .TextFileSpaceDelimiter = False
        .TextFileColumnDataTypes = Array(1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1)
        .TextFileTrailingMinusNumbers = True
		.TextFileDecimalSeparator = "."
		.TextFileThousandsSeparator = ","
        .Refresh BackgroundQuery:=False
        .Delete
    End With
End Sub

Sub DeleteSheetIfExists(sheetname As String)
    Application.DisplayAlerts = False

    For i = 1 To Worksheets.Count
        If (Worksheets(i).Name = sheetname) Then
            Worksheets(i).Delete
            Exit For
        End If
    Next i
End Sub

Function ResetSheet(sheetname As String) As Worksheet
    DeleteSheetIfExists sheetname

    ' create worksheet
    Dim ws As Worksheet
    Set ws = Worksheets.Add()
    ws.Name = sheetname
    
    Set ResetSheet = ThisWorkbook.Sheets(sheetname)
End Function
