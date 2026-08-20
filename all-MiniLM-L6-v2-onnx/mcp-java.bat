@echo off
:: CMD 코드 페이지를 UTF-8로 변경
chcp 65001 > nul

title MCP Server - Qdrant Java/Web (Port 8001)

:: Python 입출력을 UTF-8로 강제
set PYTHONUTF8=1

set check_compatibility=False
set FASTMCP_SERVER_HOST=0.0.0.0
set FASTMCP_SERVER_PORT=8001

set QDRANT_URL=http://localhost:6333
set COLLECTION_NAME=uptrio-web-code
set EMBEDDING_MODEL=sentence-transformers/all-MiniLM-L6-v2

set TOOL_STORE_DESCRIPTION=Java, PostgreSQL, AngularJS, ECharts 관련 코드 저장. information에는 자연어 설명, metadata.code에 스니펫 저장.
set TOOL_FIND_DESCRIPTION=Java, SQL 쿼리, Frontend(AngularJS/ECharts) 스니펫 검색.

set FASTEMBED_CACHE_PATH=D:\ProgramTools\MyModels
set HF_HOME=D:\ProgramTools\MyModels

:: 폐쇄망 전용 오프라인 설정
set HF_HUB_OFFLINE=1
set TRANSFORMERS_OFFLINE=1

echo Starting Qdrant Java MCP Server...
mcp-server-qdrant --transport sse