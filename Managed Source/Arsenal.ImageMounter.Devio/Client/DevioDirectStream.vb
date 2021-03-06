﻿
''''' DevioDirectStream.vb
''''' Client side component for use with devio proxy services provider objects created
''''' directly within the same process. This could be useful for example for directly
''''' examining virtual disk contents supplied by a proxy provider object directly in an
''''' application.
''''' 
''''' Copyright (c) 2012-2016, Arsenal Consulting, Inc. (d/b/a Arsenal Recon) <http://www.ArsenalRecon.com>
''''' This source code and API are available under the terms of the Affero General Public
''''' License v3.
'''''
''''' Please see LICENSE.txt for full license terms, including the availability of
''''' proprietary exceptions.
''''' Questions, comments, or requests for clarification: http://ArsenalRecon.com/contact/
'''''

Imports Arsenal.ImageMounter.Devio.Server.GenericProviders

Namespace Client

    ''' <summary>
    ''' Base class for classes that implement Stream for client side of Devio protocol.
    ''' </summary>
    Public Class DevioDirectStream
        Inherits DevioStream

        Public ReadOnly Property Provider As IDevioProvider

        Protected ownsProvider As Boolean

        ''' <summary>
        ''' Initiates a new instance with supplied provider object.
        ''' </summary>
        Public Sub New(provider As IDevioProvider, ownsProvider As Boolean)
            MyBase.New(provider.ToString(), Not provider.CanWrite)

            Me._Provider = provider
            Me.ownsProvider = ownsProvider
            MyBase.Size = provider.Length
        End Sub

        Public Overrides Function Read(buffer() As Byte, offset As Integer, count As Integer) As Integer
            Dim bytesread = _Provider.Read(buffer, offset, count, Position)
            If bytesread > 0 Then
                Position += bytesread
            End If
            Return bytesread
        End Function

        Public Overrides Sub Write(buffer() As Byte, offset As Integer, count As Integer)
            Dim byteswritten = _Provider.Write(buffer, offset, count, Position)
            If byteswritten > 0 Then
                Position += byteswritten
            End If
            If byteswritten <> count Then
                If byteswritten > 0 Then
                    Throw New IOException("Not all data were written")
                Else
                    Throw New IOException("Write error")
                End If
            End If
        End Sub

        Public Overrides Sub Close()

            If ownsProvider Then
                _Provider?.Dispose()
            End If

            _Provider = Nothing

            MyBase.Close()

        End Sub

    End Class

End Namespace
