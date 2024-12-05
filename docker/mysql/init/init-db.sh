#!/bin/bash
sleep 30s

/opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 123456 -i /docker-entrypoint-initdb.d/init.sql