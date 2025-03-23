from elasticsearch import Elasticsearch, helpers
import sys


index_name = "autocomplete_index";
elastic_client = Elasticsearch(
    hosts=[{
        'host': 'localhost',
        'port': 9200,
        'scheme': 'http'
    }]
);


# Create the index with autocomplete settings and mappings
def create_index():
    """
    Creates an Elasticsearch index with autocomplete settings and mappings.
    """
    index_body = {
        "settings": {
            "analysis": {
                "filter": {
                    "autocomplete_filter": {
                        "type": "edge_ngram",
                        "min_gram": 2,
                        "max_gram": 20
                    }
                },
                "analyzer": {
                    "autocomplete_analyzer": {
                        "type": "custom",
                        "tokenizer": "standard",
                        "filter": [
                            "lowercase",
                            "autocomplete_filter"
                        ]
                    },
                    "search_analyzer": {
                        "type": "custom",
                        "tokenizer": "standard",
                        "filter": ["lowercase"]
                    }
                }
            }
        },
        "mappings": {
            "properties": {
                "suggest_field": {
                    "type": "text",
                    "analyzer": "autocomplete_analyzer",
                    "search_analyzer": "search_analyzer"
                }
            }
        }
    }

    try:
        if not elastic_client.indices.exists(index=index_name):
            elastic_client.indices.create(index=index_name, body=index_body)
            print(f"Index '{index_name}' created successfully.")
        else:
            print(f"Index '{index_name}' already exists.")
    except Exception as error:
        print(f"An error occurred while creating the index: {error}")
        sys.exit(1)

def populate_index():
    """
    Indexes data from a file into the specified Elasticsearch index.
    """
    file_path = 'english_words.txt'

    try:
        with open(file_path, 'r') as file:
            bulk_data = (
                {
                    "_index": index_name,
                    "_source": {
                        "suggest_field": line
                    }
                }
                for line in file
            )
            helpers.bulk(elastic_client, bulk_data)
        print("Data successfully indexed.")
    except Exception as error:
        print(f"An error occurred during indexing: {error}")
        sys.exit(1)


# Create the index
create_index()

# Populate the index with data
populate_index()