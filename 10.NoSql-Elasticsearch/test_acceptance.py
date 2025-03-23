import pytest
from elasticsearch import Elasticsearch
from init import create_index, populate_index, index_name

@pytest.fixture(scope="module")
def elastic_client():
    client = Elasticsearch(
        hosts=[{
            'host': 'localhost',
            'port': 9200,
            'scheme': 'http'
        }]
    )
    yield client
    if client.indices.exists(index=index_name):
        client.indices.delete(index=index_name)

def test_create_index(elastic_client):
    create_index()
    assert elastic_client.indices.exists(index=index_name), "Index was not created"

def test_populate_index(elastic_client):
    create_index()
    populate_index()
    response = elastic_client.search(index=index_name, body={"query": {"match_all": {}}})
    assert response['hits']['total']['value'] > 0, "No data was indexed"