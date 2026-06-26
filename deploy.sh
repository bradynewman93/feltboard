#!/bin/bash
set -e

dotnet publish DesiringGodCrawler/DesiringGodCrawler.csproj -c Release -r linux-x64 --self-contained false
dotnet publish ArticleParser/ArticleParser.csproj -c Release -r linux-x64 --self-contained false

cd Infrastructure && cdk deploy --profile personal --require-approval never
